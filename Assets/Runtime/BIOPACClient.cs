using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class BIOPACClient : Singleton<BIOPACClient>
{
    public event Action<Status> ConnectionStatusChange;
    public enum Status
    {
        Disconnected,
        Connecting,
        Connected,
        Receving,
    }
    
    private string _ipAddress = "127.0.0.1";
    private int _port = 55555;
    private Status _connectionStatus;

    private TcpClient _client;
    private byte[] _receivedBytes;
    private int _dataBufferSize = 49152;


    public Status ConnectionStatus
    {
        get => _connectionStatus;
        set
        {
            _connectionStatus = value;
            ConnectionStatusChange?.Invoke(_connectionStatus);
        }
    }

    public string IPAddress
    {
        get => _ipAddress;
        set => _ipAddress = value;
    }

    public int Port
    {
        get => _port;
        set => _port = value;
    }

    void Start()
    {
        _connectionStatus = Status.Disconnected;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ConnectionStatus = Status.Connected;
    }

    public void Connect()
    {
        ConnectionStatus = Status.Connecting;
        _client = new TcpClient();
        
        ConsoleDebugger.Instance.Log($"Connecting to Server IP:{IPAddress} // PORT:{Port}");

        try
        {
            _client.Connect(_ipAddress, _port);
        }
        catch (Exception e)
        {
            ConsoleDebugger.Instance.Log("EXCEPTION: " + e.Message);
            ConnectionStatus = Status.Disconnected;
            return;
        }
        
        if (_receivedBytes == null)
        {
            _receivedBytes = new byte[_dataBufferSize];
        }
        if (_client.Connected)
        {
            ConnectionStatus = Status.Connected;
            ConsoleDebugger.Instance.Log($"BIOPACClient connected");
            var stream = _client.GetStream();
            stream.BeginRead(_receivedBytes, 0, _receivedBytes.Length, ReceiveCallback, null);
        }
    }
    public void Connect(string ipAddress, int port)
    {
        IPAddress = ipAddress;
        Port = port;
        Connect();
    }
    
    //TODO Complete the receive callback messages
    //TODO And add the received messages in the screen output
    private void ReceiveCallback(IAsyncResult res)
    {
        try
        {
            var stream = _client.GetStream();
            int byteLength = stream.EndRead(res);
            if (byteLength > 0)
            {
                byte[] incomingData = new byte[byteLength];
                Array.Copy(_receivedBytes, incomingData, byteLength);

                //Handling data here
                // string str = BIOPACEncoder.DecodeBIOPACString(incomingData, byteLength);
                // consoleOutput += "TCP Segment Received! Logging...";
                // LogManager.Instance.WriteLine(str /*+ "\n"*/);

                //stream.BeginRead(receivedBytes, 0, dataBufferSize, ReceiveCallback, null);
            }
            else {
                //Disconnect();
                return;
            }
            
        }
        catch (Exception ex)
        {
            Debug.LogError($"Client-side error receiving data: {ex}");
        }
    }

    public void Disconnect()
    {
        if (_client == null)
        {
            Debug.LogWarning("Trying to disconnect but client was never initialized");
            return;
        }

        try
        {
            _client.Close();
        }
        catch (Exception e)
        {
            ConsoleDebugger.Instance.Log("EXEPTION: "+ e.Message);
            return;
        }

        ConnectionStatus = Status.Disconnected;
        _client = null;
        ConsoleDebugger.Instance.Log("Successfully disconnected from Server");
    }
}
