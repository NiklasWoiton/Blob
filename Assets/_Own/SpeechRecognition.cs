using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SpeechRecognition : MonoBehaviour, SCL_IClientSocketHandlerDelegate
{
    private SCL_SocketServer socketServer;
    private readonly object valueLock = new object();
    private float value;

    public int port = 5000;

    public delegate void SpeechResult(string text);
    public static SpeechResult OnSpeechResult;

    void OnEnable()
    {
        SCL_IClientSocketHandlerDelegate clientSocketHandlerDelegate = this;
        socketServer = new SCL_SocketServer(clientSocketHandlerDelegate, 5, "\n", port, Encoding.UTF8);
        socketServer.StartListeningForConnections();
    }

    void OnDisable()
    {
        if (socketServer != null)
        {
            socketServer.Cleanup();
            socketServer = null;
        }
    }

    public void ClientSocketHandlerDidReadMessage(SCL_ClientSocketHandler handler, string message)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(ThisWillBeExecutedOnTheMainThread(message));
    }

    public IEnumerator ThisWillBeExecutedOnTheMainThread(string message)
    {
        if (OnSpeechResult != null)
        {
            OnSpeechResult(message);
        }
        else
        {
            Debug.Log(message);
        }

        yield return null;
    }
}