using UnityEngine;

using Unosquare.Net;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

public class WebSocketsHandler : WebSocketsServer
{
    public WebSocketsHandler()
        : base(true, 0)
    {
        // placeholder
    }

    /// <summary>
    /// Called when this WebSockets Server receives a full message (EndOfMessage) form a WebSockets client.
    /// </summary>
    protected override void OnMessageReceived(WebSocketContext context, byte[] rxBuffer, WebSocketReceiveResult rxResult)
    {
        ClientToServerMessageWrapper parsedMessage = ClientToServerMessageWrapper.Parser.ParseFrom(rxBuffer);
        MessageHandler.incoming.Enqueue(parsedMessage);

        //string message = Encoding.GetString(rxBuffer);
        //foreach (var ws in this.WebSockets)
        //{
        //    //if (ws != context)
        //        this.Send(ws, message);
        //}
    }

    /// <inheritdoc/>
    public override string ServerName { get { return "Chat Server"; } }

    /// <summary>
    /// Called when this WebSockets Server accepts a new WebSockets client.
    /// </summary>
    protected override void OnClientConnected(WebSocketContext context)
    {
        this.Send(context, "Welcome to the chat room!");

        foreach (var ws in this.WebSockets)
        {
            if (ws != context)
                this.Send(ws, "Someone joined the chat room.");
        }
    }

    /// <summary>
    /// Called when this WebSockets Server receives a message frame regardless if the frame represents the EndOfMessage.
    /// </summary>
    protected override void OnFrameReceived(WebSocketContext context, byte[] rxBuffer, WebSocketReceiveResult rxResult)
    {
        return;
    }

    /// <summary>
    /// Called when the server has removed a WebSockets connected client for any reason.
    /// </summary>
    protected override void OnClientDisconnected(WebSocketContext context)
    {
        this.Broadcast("Someone left the chat room.");
    }

    public void BroadcastString(string payload)
    {
        Broadcast(payload);
    }
}