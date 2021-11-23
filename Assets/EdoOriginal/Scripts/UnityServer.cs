using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class UnityServer : MonoBehaviour
{
    private static TcpListener server;
    TcpClient client;
    //IEnumerator doClientConnection = null;

    [SerializeField] private Text consoleText;
    [SerializeField] private Text clientListText;
    [SerializeField] private Button startSendingButton;
    [SerializeField] private float sendInterval = 0.01f;
    private string clientName;

    private bool updateClientList;

    private string consoleOutput;

    public string ipAddress = "127.0.0.1";
    public int port = 55010;

    [SerializeField] private static int dataBufferSize = 49152;

    public int syncFrequency = 60;
    private float syncTimer = 0.0f;

    [SerializeField] private static int maxClients = 8;

    //List<byte> byteBuffer = new List<byte>();

    private static byte[] receivedBytes;

    public bool sceneObjectSyncing = false;

    private static Dictionary<int, ConnectedClient> clients = new Dictionary<int, ConnectedClient>();

    public ConcurrentDictionary<int, string> clientNames = new ConcurrentDictionary<int, string>();

    private void Start()
    {
        InitializeClientDictionary();

        receivedBytes = new byte[dataBufferSize];

        IPAddress ip = IPAddress.Parse(ipAddress);
        server = new TcpListener(ip, port);
        server.Start();
        consoleText.text = "SERVER STARTED";
        server.BeginAcceptTcpClient(new AsyncCallback(ClientConnected), null);
        consoleText.text = "WAITING FOR CLIENT TO CONNECT...";

        clientListText.gameObject.SetActive(true);

        startSendingButton.gameObject.SetActive(true);
        startSendingButton.onClick.AddListener(StartSendingButtonPressed);
    }

    private void StartSendingButtonPressed()
    {
        StartCoroutine(SendCoroutine(sendInterval));
    }

    private void InitializeClientDictionary() {
        for (int i = 1; i <= maxClients; i++) {
            ConnectedClient cc = new ConnectedClient(i);
            clients.Add(i, cc);
            cc.SetUnityServer(this);

        }
    }

    private void ClientConnected(IAsyncResult res) //called only when a new client connects
    {
        
        TcpClient newClient = server.EndAcceptTcpClient(res);

        server.BeginAcceptTcpClient(new AsyncCallback(ClientConnected), null); //keep listening for connections, so other clients can connect

        consoleOutput = "Incoming connection from: " + newClient.Client.RemoteEndPoint;

        for (int i = 1; i <= maxClients; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(newClient);
                return;
            }
        }

        consoleOutput = "A client is trying to connect, but the client limit (" + maxClients +") has already been reached";
    }

    public void WriteToConsole(string str) {
        consoleOutput = str;
    }

    public void UpdateClientName(int clientId, string cName) {
        clients[clientId].myName = cName;
        updateClientList = true;
    }

    //NEWSTUFF
    private void OnDestroy()
    {
        foreach (KeyValuePair<int, ConnectedClient> c in clients)
        {
            if (c.Value.tcp.socket != null)
            {
                c.Value.tcp.Disconnect();
            }
        }
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(consoleOutput))
        {
            consoleText.text = consoleOutput;
            consoleOutput = "";
        }

        if (updateClientList)
        {
            string clientListString = "";

            foreach (KeyValuePair<int, ConnectedClient> c in clients) {
                if (c.Value.tcp.socket != null)
                {
                    clientListString += c.Key + ") " + c.Value.GetClientName() + "(ID: " + c.Value.GetClientID() + ")\n";
                }
            }

            clientListText.text = clientListString;

            updateClientList = false;
        }

    }

    private IEnumerator SendCoroutine(float interval)
    {
        yield return new WaitForSeconds(interval);

        if (clients[1].tcp.socket == null)
        {
            Debug.LogError("No client at index 1");
        }
        else
        {
            if (ParserController.Instance.AdvanceToNextLine())
            {
                SendMessageToClient(1, BIOPACEncoder.EncodeBIOPACString(ParserController.Instance.CurrentLine));
            }
            else
            {
                Debug.Log("Input file has ended.");
            }
        }

        StartCoroutine(SendCoroutine(sendInterval));
    }

    //METHODS FOR SENDING MESSAGES TO CLIENTS
    public void SendMessageToClient(int clientId, byte[] msg) //sends message to a single client with ID = clientId
    {
        if (clients[clientId].tcp.socket != null)
        {
            clients[clientId].tcp.SendMessage(msg);
        }
    }

    public void SendMessageToAllClients(byte[] msg) //sends message to a all clients
    {
        for (int n = 1; n <= clients.Count; n++)
        {
            if (clients[n].tcp.socket != null)
            {
                clients[n].tcp.SendMessage(msg);
            }
        }
    }

    public void SendMessageToAllClients(byte[] msg, int exceptClient) //alternative overload method that sends a message to all clients EXCEPT client with ID = exceptClient
    {
        for (int n = 1; n <= clients.Count; n++)
        {
            if (clients[n].tcp.socket != null && n != exceptClient)
            {
                clients[n].tcp.SendMessage(msg);
            }
        }
    }
}
