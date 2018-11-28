using System;
using System.IO;

namespace namedpipetest
{
    class Program
    {
        static void Main(string[] args)
        {
	  // adjust whether to behave as a server or as a client based on command line flags
	  if (args.Length == 1) {
	     Client(args[0]);
	  } else {
  	     Server();
	  }
          Console.WriteLine("Done!");
	  Environment.Exit(0);
        }

	static void Client(string arg)
	{
	   Console.WriteLine("Should be running as a client!");
	}

	static void Server()
	{
	   Console.WriteLine("Should be running as a server!");
	}
    }
}
