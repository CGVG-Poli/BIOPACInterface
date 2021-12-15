using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;

public class SyncManager : Singleton<SyncManager>
{
    void Start()
    {
        BIOPACInterfaceMessageHandler.Instance.SyncMessage += OnSyncMessage;
    }

    private void OnSyncMessage(ClientServerSyncMessage message, NetPeer peer)
    {
        if (message.ClientTime == null && message.ServerTime != null)
        {
            ConsoleDebugger.Instance.Log($"Client Received Server Time: {message.ServerTime}. Now sending its time.");
            message.ClientTime = new SimpleTime(DateTime.Now);
            peer.Send(BIOPACInterfaceMessageHandler.Instance.PacketProcessor.Write(message), DeliveryMethod.ReliableOrdered);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
