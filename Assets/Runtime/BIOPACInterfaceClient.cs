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
    private NetManager _netClient;
    private NetPeer _serverPeer;
    private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();
    // Start is called before the first frame update
    void Start()
    {
        
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

    public void StartClient()
    {
        _netClient = new NetManager(this);
        _netClient.UnconnectedMessagesEnabled = true;
        _netClient.UpdateTime = 15;
        _netClient.Start();
        SubscribeToMessages();
        ConsoleDebugger.Instance.Log("Starting BIOPACInterface Client");
    }
    private void SubscribeToMessages()
    {
        _netPacketProcessor.RegisterNestedType<SimpleTime>(() => new SimpleTime()); // We need to pass the constructor when it's not a struct.
        _netPacketProcessor.SubscribeReusable<ClientServerSyncMessage, NetPeer>(OnCLientServerSyncMessage);
    }

    private void OnCLientServerSyncMessage(ClientServerSyncMessage arg1, NetPeer arg2)
    {
        ConsoleDebugger.Instance.Log($"Recevied ClientServerSyncMessage ClientTime:{arg1.ClientTime} // ServerTime:{arg1.ServerTime}");

    }
    
    public void OnPeerConnected(NetPeer peer)
    {
        ConsoleDebugger.Instance.Log("[CLIENT] Connected to server");
        _serverPeer = peer;

        ClientServerSyncMessage message = new ClientServerSyncMessage
        {
            ClientTime = new SimpleTime(DateTime.Now),
            ServerTime = new SimpleTime(),
        };
        
        ConsoleDebugger.Instance.Log($"[CLIENT] Sending first message to client: Start {message.ClientTime}");

        _serverPeer.Send(_netPacketProcessor.Write(message), DeliveryMethod.ReliableOrdered);
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
        reader.Recycle();
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
        throw new System.NotImplementedException();
    }
}
