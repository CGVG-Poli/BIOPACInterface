using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class BIOPACInterfaceServer : Singleton<BIOPACInterfaceServer>, INetEventListener, INetLogger
{
    public event Action<ClientServerSyncMessage, bool> ReceivedClientServerSyncMessage; 
    
    [SerializeField] private int _connectionPort = 5000; 
    private NetManager _netServer;
    private Dictionary<string, NetPeer> _peers;
    private NetDataWriter _dataWriter;
    private BIOPACInterfaceMessageHandler _messageHandler;
    private NetPacketProcessor _netPacketProcessor;

    public Dictionary<string, NetPeer> Peers => _peers;

    void Start()
    {
        _peers = new Dictionary<string, NetPeer>();
        _messageHandler = BIOPACInterfaceMessageHandler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        _netServer?.PollEvents();
    }

    private void OnDestroy()
    {
        NetDebug.Logger = null;
        if (_netServer != null)
            _netServer.Stop();
    }

    public void StartServer(int port)
    {
        ConsoleDebugger.Instance.Log($"Starting BIOPACInterfaceServer on port {port}");
        NetDebug.Logger = this;
        _dataWriter = new NetDataWriter();
        _netServer = new NetManager(this);
        _netServer.Start(port);
        _netServer.BroadcastReceiveEnabled = true;
        _netServer.UpdateTime = 15;

        SubscribeToMessages();
    }
    
    public void SendMessageToClient<T>(T message) where T : BaseMessage, new()
    {
        T messageCasted = message as T;
        _peers.First().Value.Send(_netPacketProcessor.Write(messageCasted), DeliveryMethod.ReliableOrdered);
    }

    public void StartServer()
    {
        StartServer(_connectionPort);
    }

    public void StopServer()
    {
        NetDebug.Logger = null;
        if (_netServer != null)
            _netServer.Stop();
        
        ConsoleDebugger.Instance.Log("Stopped BIOPACInterface Server");
    }

    #region Network Callbacks

    public void OnPeerConnected(NetPeer peer)
    {
        ConsoleDebugger.Instance.Log($"BIOPACInterfaceClient connected with ID {peer.Id}");
        _peers[peer.Id.ToString()] = peer;
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        ConsoleDebugger.Instance.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
        if(!_peers.ContainsKey(peer.Id.ToString()))
        {
            ConsoleDebugger.Instance.Log($"Peer with ID {peer.Id} was not present in previous connections");
            return;
        }
            
        _peers.Remove(peer.Id.ToString());
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        ConsoleDebugger.Instance.Log("[SERVER] error " + socketError);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        _netPacketProcessor.ReadAllPackets(reader, peer);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.Broadcast)
        {
            ConsoleDebugger.Instance.Log("[SERVER] Received discovery request. Send discovery response");
            NetDataWriter resp = new NetDataWriter();
            resp.Put(1);
            _netServer.SendUnconnectedMessage(resp, remoteEndPoint);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        request.AcceptIfKey("sample_app");
    }

    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        Debug.LogFormat(str, args);
    }

    #endregion

    #region Message Handling

    private void SubscribeToMessages()
    {
        //_messageHandler.SubscribeToSharedMessages();
        _netPacketProcessor = new NetPacketProcessor();
        _netPacketProcessor.RegisterNestedType<SimpleTime>(() => new SimpleTime()); // We need to pass the constructor when it's not a struct.
        _netPacketProcessor.SubscribeReusable<ClientServerSyncMessage, NetPeer>(OnClientServerSyncMessage);
    }

    private void OnClientServerSyncMessage(ClientServerSyncMessage message, NetPeer peer)
    {
        ReceivedClientServerSyncMessage?.Invoke(message, true);
    }

    #endregion
}
