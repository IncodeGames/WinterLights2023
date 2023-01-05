using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using J38.WebSockets.Client;

public class SendReceive : MonoBehaviour
{
    string url = "wss://websocket-testeroo.glitch.me";
    private WebSocketClient webSocketClient;

    void Start()
    {
        webSocketClient = new WebSocketClient(url);

        webSocketClient.OnConnected += () =>
        {
            webSocketClient.Send("{\"command\":\"listen\"}");
        };

        webSocketClient.OnReceived += (string message) =>
        {
            Debug.Log(message);
        };

    }

    void OnDestroy()
    {
        webSocketClient.Disconnect();
    }
}
