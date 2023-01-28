using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using J38.WebSockets.Client;

public class ClientInterface : MonoBehaviour
{
    [SerializeField] private Transform turbTransform;

    private const string url = "wss://websocket-testeroo.glitch.me";
    private WebSocketClient webSocketClient;

    private Vector3 packetPosition;

    struct Packet
    {
        public int id;
        public float x;
        public float y;
    }

    void Start()
    {
        webSocketClient = new WebSocketClient(url);

        webSocketClient.OnConnected += () =>
        {
            webSocketClient.Send("{\"command\":\"listen\"}");
        };

        webSocketClient.OnReceived += ProcessMessageReceived;
    }

    private void ProcessMessageReceived(string message)
    {
        Packet packet = JsonUtility.FromJson<Packet>(message);
        Debug.LogFormat("x: {0}, y: {1}", packet.x, packet.y);
        packetPosition.x = packet.x;
        packetPosition.y = 1 - packet.y;
        packetPosition.z = 5;
        Vector3 turbWorldPosition = Camera.main.ViewportToWorldPoint(packetPosition);
        Debug.LogFormat("x: {0}, y: {1}", turbWorldPosition.x, 1 - turbWorldPosition.y);
        turbWorldPosition.z = 5;
        turbTransform.position = turbWorldPosition;
    }

    void OnDestroy()
    {
        webSocketClient.Disconnect();
    }
}
