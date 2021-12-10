using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.UI;

public class BIOPACInterfaceDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
