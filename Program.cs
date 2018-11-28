using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace namedpipetest
{

    class Program
    {
        const string pipeName = "PipeSample";

	static void Main(string[] args)
        {
	  // adjust whether to behave as a server or as a client based on command line flags
	  if (args.Length == 1) {
	     Client(args[0]);
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
	      using(var strm = new NamedPipeServerStream(pipeName, PipeDirection.Out, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous)) {
	      	// Presumably this doesn't block, it just goes about its business, it's the `await` that makes it block
		Console.Write("Created pipe, now waiting for connection");		
		await strm.WaitForConnectionAsync();
		Console.WriteLine("Connected!");
		Task nowait = ResponseToRequestAsync(strm, ++clientId);
	      }
	  }
	}

	private static async Task ResponseToRequestAsync(NamedPipeServerStream stream, int clientId)
	{
	    Console.WriteLine($"Connection request #{clientId} received, spinning off an async Task to deal with it");
	    using (var writer = new StreamWriter(stream)) {
	       Console.Write("Enter message: ");
	       var message = Console.ReadLine();
	       writer.WriteLine(message);
	    }
	    Console.WriteLine($"Connection #{clientId} serviced");
	}

    }
}
