using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using J38.WebSockets.Client;

public class ClientInterface : MonoBehaviour
{
    [SerializeField] private Transform turbTransform;
    [SerializeField] private GameObject[] ufoPrefabs;
    
    private Dictionary<int, GameObject> clientUFOLookup = new Dictionary<int, GameObject>();

    private const string url = "wss://ceti-wl-2023.glitch.me";
    private WebSocketClient webSocketClient;

    private Vector3 packetPosition;

    private const int MAX_CONNECTIONS = 16;

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
        if (!clientUFOLookup.ContainsKey(packet.id))
        {
            clientUFOLookup[packet.id] = Instantiate(ufoPrefabs[Random.Range(0, ufoPrefabs.Length)], Vector3.zero, Quaternion.identity);
        }

        Debug.LogFormat("x: {0}, y: {1}", packet.x, packet.y);
        packetPosition.x = packet.x;
        packetPosition.y = 1 - packet.y;
        packetPosition.z = 6;
        Vector3 turbWorldPosition = Camera.main.ViewportToWorldPoint(packetPosition);
        Debug.LogFormat("x: {0}, y: {1}", turbWorldPosition.x, 1 - turbWorldPosition.y);
        turbWorldPosition.z = 6;
        turbTransform.position = turbWorldPosition;
        
        if (clientUFOLookup.ContainsKey(packet.id))
        {
            clientUFOLookup[packet.id].transform.position = turbWorldPosition;
        }
    }

    void OnDestroy()
    {
        webSocketClient.Disconnect();
    }
}
