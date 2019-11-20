using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

using StreamJsonRpc;

namespace namedpipetest
{

    class Program
    {
        const string pipeName = "PipeSample";

		static void Main(string[] args)
        {
		  	// adjust whether to behave as a server or as a client based on command line flags
	  		if (args.Length == 1) {
	     		//Client(args[0]);
	     		ClientAsync(args[0]).GetAwaiter().GetResult();
	  		} else {
  	     		// Server();
	     		ServerAsync().GetAwaiter().GetResult();
	  		}
          	Console.WriteLine("Done!");
	  		Environment.Exit(0);
        }

		static void Client(string server)
		{
	   		Console.WriteLine("Running as client!");
	   		// Ignoring :args server: for now, assuming both processes live in the same box
	   		using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.In)) {
	   	 		Console.Write($"Connecting to {server}");
		 		client.Connect();
		 		Console.WriteLine("Connected");

		 		using (var reader = new StreamReader(client)) {
		       		var message = reader.ReadLine();
		       		Console.WriteLine($"Received message: [{message}]");
		 		}
	   		}
		}

		static async Task ClientAsync(string server)
		{
	   		Console.WriteLine("Running as client, and asynchronously!");
	   		using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous)) {
	       		await client.ConnectAsync();
	       		Console.WriteLine("Connected, sending request"); // TODO: maybe should spin up a task?
	       		var jsonRpc = JsonRpc.Attach(client);
#if false
		    	try {
	       	       int sum = await jsonRpc.InvokeAsync<int>("Add", 6, 4);
		       		Console.WriteLine($"6 + 4 = {sum}.");
	       		} catch (Exception ex) {
	           		Console.WriteLine($"There was a problem carrying out the operation: {ex.Message}");
	       		}
#else
	       		try {
	       	       	string greet = await jsonRpc.InvokeAsync<string>("Greet", "test");
		       		Console.WriteLine($"Received response [{greet}].");
	       		} catch (Exception ex) {
	           		Console.WriteLine($"There was a problem carrying out the operation: {ex.Message}");
	       		}
#endif
	       		Console.WriteLine("Terminating stream");
	   		}
		}

		static void Server()
		{
	   		Console.WriteLine("Running as server!");
	   		using (NamedPipeServerStream ps = new NamedPipeServerStream(pipeName,
											       					    PipeDirection.Out,
								       									NamedPipeServerStream.MaxAllowedServerInstances,
								       									PipeTransmissionMode.Byte)) {
	    		Console.Write("Waiting for connection...");
	    		ps.WaitForConnection();
	    		Console.WriteLine("Connected!");

	    		using (var writer = new StreamWriter(ps)) {
	    	  		Console.Write("Enter message: ");
		  			var message = Console.ReadLine();
		  			writer.WriteLine(message);
	    		}
			}
		}

		static async Task ServerAsync()
		{
	  		int clientId = 0;
	  		while (true) {
	      		Console.WriteLine("Waiting for client to make a connection");
	      		var strm = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
	      		// Presumably this doesn't block, it just goes about its business, it's the `await` that makes it block
	      		Console.Write("Created pipe, now waiting for connection");		
	      		await strm.WaitForConnectionAsync();
	      		Console.WriteLine("Connected!");
	      		Task nowait = ResponseToRequestAsync(strm, ++clientId);
	  		}
		}

		private static async Task ResponseToRequestAsync(NamedPipeServerStream strm, int clientId)
		{
	    	Console.WriteLine($"Connection request #{clientId} received, spinning off an async Task to deal with it");
	    	var jsonRpc = JsonRpc.Attach(strm, new Program());
	    	Console.WriteLine($"JSON-RPC listener attached to #{clientId}, waiting for requests...");

	    	await jsonRpc.Completion; // is this per-request, or is there a notion of "session"?
	    	Console.WriteLine($"Connection #{clientId} terminated.");
		}

		public int Add(int a, int b)
		{
	    	Console.WriteLine($"Received request to add {a} and {b}.");
	    	return a+b;
		}

		public string Greet(string whom)
		{
	    	Console.WriteLine($"Received request to greet {whom}.");
	    	return $"Hello, world: {whom}!";
		}

    }
}
