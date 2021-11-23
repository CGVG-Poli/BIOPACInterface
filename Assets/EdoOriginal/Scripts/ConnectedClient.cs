using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

//this class is not a client, is a structure that is used by the SERVER (!!!!) to store client information, listen to a particular client, and also send data to that particular client.
public class ConnectedClient 
{
    public int myID;
    public string myName;
    public TCP tcp;
    public static int dataBufferSize = 49152;
    public static UnityServer unityServer;
    public bool syncSceneObjects;

    public ConnectedClient(int clientID) {
        myID = clientID;
        tcp = new TCP(myID);
    }

    public void SetUnityServer(UnityServer us) {
        unityServer = us;
    }

    public string GetClientName() {
        return myName;
    }

    public int GetClientID() {
        return myID;
    }

    public void SetName(string str) {
        myName = str;
    }

    public class TCP
    {
        public TcpClient socket;
        private readonly int ID;
        private byte[] receiveBuffer;
        private NetworkStream stream;

        public TCP(int id) {
            ID = id;
        }

        public void Connect(TcpClient socket)
        {
            this.socket = socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();

            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        //NEWSTUFF
        public void Disconnect()
        {
            unityServer.WriteToConsole("Client with ID # " + ID + " disconnected, cleaning up client slot...");
            ThreadManager.ExecuteOnMainThread(() => unityServer.UpdateClientName(ID, ""));
            socket.Close();
            this.socket = null;

        }

        private void ReceiveCallback(IAsyncResult res)
        {
            //the SERVER (because this is the server, despite the name) should not receive anything
        }

        public void SendMessage(byte[] msg) {
            stream.Write(msg, 0, msg.Length);
        }


    }
}
