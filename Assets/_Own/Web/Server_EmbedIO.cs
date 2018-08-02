using UnityEngine;

using Unosquare.Swan;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

public class Server_EmbedIO : MonoBehaviour
{
    private WebServer server;
    private WebSocketsHandler webSocketsHandler;

    public int port = 80;

    void OnEnable()
    {
        string path = Application.streamingAssetsPath + "/EmbeddedWebserver_StaticFiles";
        Debug.Log("Starting WebServer, serving folder '" + path + "'...");

        Terminal.OnLogMessageReceived += OnLogMessageReceived;

        server = new WebServer(port, Unosquare.Labs.EmbedIO.Constants.RoutingStrategy.Regex);
        server.RegisterModule(new StaticFilesModule(path));
        // The static files module will cache small files in ram until it detects they have been modified.
        server.Module<StaticFilesModule>().UseRamCache = true;
        server.Module<StaticFilesModule>().DefaultExtension = ".html";
        // We don't need to add the line below. The default document is always index.html.
        //server.Module<Modules.StaticFilesWebModule>().DefaultDocument = "index.html";
        server.RegisterModule(new WebApiModule());
        server.Module<WebApiModule>().RegisterController<ExperimentalApi>();
        server.RegisterModule(new WebSocketsModule());
        webSocketsHandler = new WebSocketsHandler();
        server.Module<WebSocketsModule>().RegisterWebSocketsServer<WebSocketsHandler>("/ws", webSocketsHandler);

        server.RunAsync();
    }

    void OnDisable()
    {
        Debug.Log("Disposing WebServer...");
        server.Dispose();
        server = null;
        webSocketsHandler.Dispose();
        webSocketsHandler = null;
    }

    void OnLogMessageReceived(object sender, LogMessageReceivedEventArgs args)
    {
        UnityMainThreadDispatcher.Instance().Enqueue((() =>
        {
            //if ((args.MessageType & (LogMessageType.None | LogMessageType.Trace | LogMessageType.Debug | LogMessageType.Info)) != 0)
            //{
            //    Debug.Log(args.Message);
            //}
            //else
            if ((args.MessageType & (LogMessageType.Warning)) != 0)
            {
                Debug.LogWarning(args.Message);
            }
            else if ((args.MessageType & (LogMessageType.Error | LogMessageType.Fatal)) != 0)
            {
                Debug.LogError(args.Message);
            }
        }));
    }

    void BroadcastToConnectedClients()
    {
        webSocketsHandler.BroadcastString("Brotkast");
    }
}
