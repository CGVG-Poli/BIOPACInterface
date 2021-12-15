using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class MainManager : Singleton<MainManager>
{
    protected override void Awake()
    {
        base.Awake();
        Application.runInBackground = true;
    }
    void Update()
    {
        ThreadManager.UpdateMain();
        
        //DEBUG FUNCTIONS
        if (Input.GetKeyDown(KeyCode.C))
        {
            BIOPACInterfaceClient.Instance.StartClient();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            BIOPACInterfaceServer.Instance.StartServer();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            NetPeer peer = BIOPACInterfaceServer.Instance.Peers.First().Value;
            NetDataWriter writer = new NetDataWriter();                 // Create writer class
            writer.Put("Hello client!");                                // Put some string
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }
}
