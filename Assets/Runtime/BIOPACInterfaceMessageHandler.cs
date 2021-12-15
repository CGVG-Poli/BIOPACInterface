using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class BIOPACInterfaceMessageHandler : Singleton<BIOPACInterfaceMessageHandler>
{
    public event Action<ClientServerSyncMessage, NetPeer> SyncMessage;
    
    private NetPacketProcessor _netPacketProcessor;

    public NetPacketProcessor PacketProcessor => _netPacketProcessor;

    public void SubscribeToSharedMessages()
    {
        _netPacketProcessor = new NetPacketProcessor();
        _netPacketProcessor.RegisterNestedType<SimpleTime>(() => new SimpleTime()); // We need to pass the constructor when it's not a struct.
        _netPacketProcessor.SubscribeReusable<ClientServerSyncMessage, NetPeer>(OnClientServerSyncMessage);
    }

    private void OnClientServerSyncMessage(ClientServerSyncMessage message, NetPeer peer)
    {
        SyncMessage?.Invoke(message,peer);
        return;
        
        if (message.ClientTime == null && message.ServerTime != null)
        {
            ConsoleDebugger.Instance.Log($"Client Received Server Time: {message.ServerTime}. Now sending its time.");
            message.ClientTime = new SimpleTime(DateTime.Now);
            peer.Send(_netPacketProcessor.Write(message), DeliveryMethod.ReliableOrdered);
            return;
        }

        if (message.ClientTime != null && message.ServerTime != null)
        {
            ConsoleDebugger.Instance.Log($"Recived both server and Client Time. Computing Sync");
            SimpleTime serverNewTime = new SimpleTime(DateTime.Now);
            double rountTripTime = (serverNewTime - message.ServerTime).TotalMilliseconds;
            double networkLatency = rountTripTime / 2f;

            double clientDelta = (message.ClientTime - message.ServerTime).TotalMilliseconds + networkLatency;
            ConsoleDebugger.Instance.Log($"Recived both server and Client Time. Clocks desync is {clientDelta}");
            return;
        }
    }
}
