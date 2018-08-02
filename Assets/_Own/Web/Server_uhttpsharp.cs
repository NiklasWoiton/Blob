using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using uhttpsharp;
using uhttpsharp.Handlers;
using uhttpsharp.Handlers.Compression;
using uhttpsharp.Listeners;
using uhttpsharp.ModelBinders;
using uhttpsharp.RequestProviders;

public class Server_uhttpsharp : MonoBehaviour
{
    private HttpServer httpServer;

    void OnEnable()
    {
        string path = Application.dataPath + "/_StaticHTTPFiles/";
        Debug.Log("Starting HttpServer, serving folder '" + path + "'...");
        FileHandler.HttpRootDirectory = path;

        httpServer = new HttpServer(new HttpRequestProvider());
        httpServer.Use(new TcpListenerAdapter(new TcpListener(IPAddress.Any, 80)));

        httpServer.Use(new ExceptionHandler());
        httpServer.Use(new CompressionHandler(DeflateCompressor.Default, GZipCompressor.Default));
        httpServer.Use(new FileHandler());

        //httpServer.Use(new RedirectResponse);
        httpServer.Use((context, next) =>
        {
            Debug.Log("Got Request: " + context.Request.Uri);
            return next();
        });

        httpServer.Start();
    }

    void OnDisable()
    {
        Debug.Log("Disposing HttpServer...");
        httpServer.Dispose();
        httpServer = null;
    }
}
