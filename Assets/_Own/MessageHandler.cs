using Google.Protobuf;

using System.Collections.Concurrent;

using UnityEngine;

public class MessageHandler : MonoBehaviour
{

    public static ConcurrentQueue<ClientToServerMessageWrapper> incoming = new ConcurrentQueue<ClientToServerMessageWrapper>();

    void Update()
    {
        ClientToServerMessageWrapper messageWrapper;
        if (incoming.TryDequeue(out messageWrapper))
        {
            Debug.Log("Received PB message: " + messageWrapper.ToString());

            switch (messageWrapper.MessageCase)
            {
                case ClientToServerMessageWrapper.MessageOneofCase.SpawnEnemy:
                    {
                        HandleMessage(messageWrapper.SpawnEnemy);
                        break;
                    }
            }
        }
    }

    private void HandleMessage(SpawnEnemy spawnEnemy)
    {
        Debug.Log("HandleMessage(SpawnEnemy): " + spawnEnemy.What + " " + spawnEnemy.Where);
    }
}
