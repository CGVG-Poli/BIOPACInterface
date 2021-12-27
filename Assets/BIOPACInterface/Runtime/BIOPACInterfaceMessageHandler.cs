using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

//TODO consider removing this class because at the moment it is useless...
public class BIOPACInterfaceMessageHandler : Singleton<BIOPACInterfaceMessageHandler>
{
    public event Action<ClientServerSyncMessage, NetPeer> SyncMessage;
    
    private NetPacketProcessor _netPacketProcessor;

    public NetPacketProcessor PacketProcessor => _netPacketProcessor;

    public void SubscribeToSharedMessages()
    {
    }
}
