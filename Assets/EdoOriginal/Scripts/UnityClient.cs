using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class UnityClient : MonoBehaviour
{
    public static UnityClient instance;
    [SerializeField] private Text consoleText;
    public string clientName;
    private int myID = -1;

    public string ipAddress = "127.0.0.1";
    public int port = 55010;

    public TcpClient client;

    [SerializeField] private static int dataBufferSize = 49152;

    private byte[] receivedBytes;

    private string consoleOutput;

    private bool updateTransforms = false;

    private bool parseMessages = false;

    private List<string> consoleLines = new List<string>();

    private void Awake()
    {
        Assert.IsNull(instance);
        instance = this;
    }

    public static UnityClient Instance
    {
        get
        {
            return instance;
        }
    }

    private void Start()
    {
        receivedBytes = new byte[dataBufferSize];
    }

    public void OpenConnection() {
        client = new TcpClient();
        client.Connect(ipAddress, port);
        if (receivedBytes == null)
        {
            receivedBytes = new byte[dataBufferSize];
        }
        if (client.Connected)
        {
            Debug.Log($"Client connect to Server IP:{ipAddress} // PORT:{port}");
            var stream = client.GetStream();
            /*
            //byte[] msg = MyEncoder.EncodeMessage(MessageType.COMMAND, CommandType.OPEN_CONNECTION_REQUEST, clientName);
            byte[] msg = BIOPACEncoder.EncodeBIOPACString("Signor server per favore mi connetta");
            stream.Write(msg, 0, msg.Length);
            
            Debug.Log($"Received first connection message bytle length is: {receivedBytes.Length} // stream DataAvailable:{stream.DataAvailable} ");
            */
            stream.BeginRead(receivedBytes, 0, receivedBytes.Length, ReceiveCallback, null);
        }
        
    }


    private void OnDestroy()
    {
        client.GetStream().Close();
        if (client.Connected) {
            client.Close();
        }
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(consoleOutput)) {

            string trueConsoleOutput = "";

            if (parseMessages)
            {
                List<string> parsedValues = ParserController.Instance.ParseRelevantFieldsToString(consoleOutput);
                for (int i = 0; i < parsedValues.Count; i++)
                {
                    trueConsoleOutput += parsedValues[i] + ";";
                }
            }
            else
            {
                trueConsoleOutput = consoleOutput;
            }

            
            consoleText.text = trueConsoleOutput;
            //consoleText.rectTransform.sizeDelta = new Vector2(consoleText.preferredWidth, consoleText.preferredHeight);
            consoleOutput = "";
            
        }

        //ThreadManager.UpdateMain();
    }

    public void SetMessageParsing(bool parseMessages)
    {
        this.parseMessages = parseMessages;
    }

    private void Disconnect()
    {
        Debug.LogError("ERROR! Cannot reach server, closing down tcp connection...");
        client.Close();
    }

    private void ReceiveCallback(IAsyncResult res)
    {
        try
        {
            var stream = client.GetStream();
            int byteLength = stream.EndRead(res);
            if (byteLength > 0)
            {
                byte[] incomingData = new byte[byteLength];
                Array.Copy(receivedBytes, incomingData, byteLength);

                //Handling data here
                string str = BIOPACEncoder.DecodeBIOPACString(incomingData, byteLength);
                consoleOutput += "TCP Segment Received! Logging...";
                LogManager.Instance.WriteLine(str /*+ "\n"*/);

                stream.BeginRead(receivedBytes, 0, dataBufferSize, ReceiveCallback, null);
            }
            else {
                Disconnect();
                return;
            }
            
        }
        catch (Exception ex)
        {
            Debug.LogError($"Client-side error receiving data: {ex}");
        }
    }
}
