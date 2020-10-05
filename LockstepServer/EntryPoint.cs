using System;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        var server = new TransferServer();

        server.Init();
        server.Run();
    }
}
