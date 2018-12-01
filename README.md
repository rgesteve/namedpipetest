# namedpipetest

A very simple test on how to use `System.IO.Pipes.NamedPipe{Server,Client}Stream` with JSON-RPC.

## To build

```
dotnet build
```

Run the server.  The server runs continously until you stop it with a Ctrl-C

```
dotnet run
```

Run the client

```
dotnet run -- <something>
```

The program detects whether it's in "server" or "client" mode by
checking whether command line arguments have been passed.
