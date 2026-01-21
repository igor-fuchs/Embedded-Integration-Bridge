using Services;
using Mapping;

class Program
{
    static void Main()
    {
        // Inicia o servidor WebSocket (para clientes web)
        // var wsServer = new WebSocketServer(Config.WEBSOCKET_PREFIX);
        // await wsServer.StartAsync();
        // Console.WriteLine($"WebSocket server iniciado em {Config.WEBSOCKET_PREFIX}");

        Console.WriteLine("Iniciando cliente OPC UA...");

        // Cria e inicia o cliente OPC UA
        var opcuaClient = new OpcUaClient(
            new Uri("opc.tcp://192.168.1.20:4840"),
            OpcNodes.NODE_IDS_TO_MONITOR
        );

        Console.WriteLine("📡 Monitorando alterações dos NodeIds. Pressione ENTER para encerrar.");
        Console.ReadLine();

        // Cleanup
        // await opcuaClient.StopAsync();
        // await wsServer.StopAsync();
        // wsServer.Dispose();

        // Console.WriteLine("Sessão encerrada.");
        

    }
}

