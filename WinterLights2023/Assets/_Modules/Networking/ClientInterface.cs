using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using J38.WebSockets.Client;

public class ClientInterface : MonoBehaviour
{
    private class User
    {
        public GameObject ufo;
        public float lastUpdateTime;
    }

    [SerializeField] private Transform turbTransform;
    [SerializeField] private GameObject[] ufoPrefabs;
    
    private Dictionary<int, User> clientUFOLookup = new Dictionary<int, User>();

    private const string url = "wss://ceti-wl-2023.glitch.me";
    private WebSocketClient webSocketClient;

    private Vector3 packetPosition;

    private const int MAX_CONNECTIONS = 20;

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
        if (clientUFOLookup.Count + 1 <= MAX_CONNECTIONS)
        {
            if (!clientUFOLookup.ContainsKey(packet.id))
            {
                clientUFOLookup[packet.id] = new User();
                clientUFOLookup[packet.id].ufo = Instantiate(ufoPrefabs[Random.Range(0, ufoPrefabs.Length)], Vector3.zero, Quaternion.identity);
                clientUFOLookup[packet.id].lastUpdateTime = Time.time;
            }
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
            clientUFOLookup[packet.id].ufo.transform.position = turbWorldPosition;
            clientUFOLookup[packet.id].lastUpdateTime = Time.time;
        }
    }

    void Update()
    {
        foreach(KeyValuePair<int, User> entry in clientUFOLookup)
        {
            if (Time.time - entry.Value.lastUpdateTime > 15.0f)
            {
                Destroy(entry.Value.ufo);
                clientUFOLookup.Remove(entry.Key);
                return;
            }
        }
    }

    void OnDestroy()
    {
        webSocketClient.Disconnect();
    }
}
