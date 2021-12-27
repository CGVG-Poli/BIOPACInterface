using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class BIOPACInterfaceClient : Singleton<BIOPACInterfaceClient>, INetEventListener
{
    public event Action<ClientServerSyncMessage, bool> ReceivedClientServerSyncMessage; 

    private NetManager _netClient;
    private NetPeer _serverPeer;
    private BIOPACInterfaceMessageHandler _messageHandler;
    private NetPacketProcessor _netPacketProcessor;

    
    void Start()
    {
        _messageHandler = BIOPACInterfaceMessageHandler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(_netClient == null)
            return;
        
        _netClient.PollEvents();
        
        var peer = _netClient.FirstPeer;
        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
            
        }
        else
        {
            _netClient.SendBroadcast(new byte[] {1}, 5000);
        }
    }

    private void OnDestroy()
    {
        _netClient?.Stop();
    }

    public void StartClient()
    {
        _netClient = new NetManager(this);
        _netClient.UnconnectedMessagesEnabled = true;
        _netClient.UpdateTime = 15;
        //TODO REMOVE LINE 
        _netClient.DisconnectTimeout = Int32.MaxValue;
        _netClient.Start();
        SubscribeToMessages();
        ConsoleDebugger.Instance.Log("Starting BIOPACInterface Client");
    }

    public void StopClient()
    {
        _netClient?.Stop();
        ConsoleDebugger.Instance.Log("Stopped BIOPACInterface Client");

    }
    
    public void SendMessageToServer<T>(T message) where T : BaseMessage, new()
    {
        T messageCasted = message as T;
        _serverPeer.Send(_netPacketProcessor.Write(messageCasted), DeliveryMethod.ReliableOrdered);
    }

    #region Network Callbacks

    public void OnPeerConnected(NetPeer peer)
    {
        ConsoleDebugger.Instance.Log("[CLIENT] Connected to server");
        _serverPeer = peer;

        ClientInformationMessage message = new ClientInformationMessage();
        message.ClientDeviceName = SystemInfo.deviceName;
        message.ClientUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
        SendMessageToServer<ClientInformationMessage>(message);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        ConsoleDebugger.Instance.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        ConsoleDebugger.Instance.Log("[CLIENT] We received error " + socketError);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        _netPacketProcessor.ReadAllPackets(reader, peer);
        //reader.Recycle();
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.BasicMessage && _netClient.ConnectedPeersCount == 0 && reader.GetInt() == 1)
        {
            ConsoleDebugger.Instance.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
            _netClient.Connect(remoteEndPoint, "sample_app");
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Message Handling

    private void SubscribeToMessages()
    {
        _netPacketProcessor = new NetPacketProcessor();
        _netPacketProcessor.RegisterNestedType<SimpleTime>(() => new SimpleTime()); // We need to pass the constructor when it's not a struct.
        _netPacketProcessor.SubscribeReusable<ClientServerSyncMessage, NetPeer>(OnClientServerSyncMessage);
    }

    private void OnClientServerSyncMessage(ClientServerSyncMessage message, NetPeer peer)
    {
        //The bool tells if the message came from client (false) or server (true)
        ReceivedClientServerSyncMessage?.Invoke(message, false);
    }

    #endregion
}
