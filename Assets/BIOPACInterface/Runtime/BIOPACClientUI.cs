using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BIOPACClientUI : Singleton<BIOPACClientUI>
{
    [SerializeField] private Button _disconnectBtn;
    [SerializeField] private Button _connectBtn;
    [SerializeField] private InputField _ipIf;
    [SerializeField] private InputField _portIf;

    [SerializeField] private Image _connectionStatusImg;
    [SerializeField] private Text _connectionStatusMessage;
    [SerializeField] private Image _messagesStatusImg;
    [SerializeField] private Text _messagesStatusMessage;

    private BIOPACClient _biopacClient;
    
    private void Start()
    {
        _biopacClient = BIOPACClient.Instance; 
        RegisterUICallbacks();
        
        _biopacClient.ConnectionStatusChange += OnConnectionStatusChange;
        OnConnectionStatusChange(BIOPACClient.Status.Disconnected);

        _ipIf.text = BIOPACClient.Instance.IPAddress;
        _portIf.text = BIOPACClient.Instance.Port.ToString();
    }

    private void OnConnectionStatusChange(BIOPACClient.Status status)
    {
        SetButtonsState(status);
        SetConnectionStatus(status);
    }

    public void SetConnectionStatus(BIOPACClient.Status status)
    {
        switch (status)
        {
            case BIOPACClient.Status.Disconnected:
                _connectionStatusImg.color = Color.red;
                _messagesStatusImg.color = Color.red;
                _connectionStatusMessage.text = "CONNECTION: " + status.ToString();
                _messagesStatusMessage.text = "MESSAGES: " + "Not Receiving";
                break;
            case BIOPACClient.Status.Connecting:
                _connectionStatusImg.color = Color.yellow;
                _messagesStatusImg.color = Color.red;
                _connectionStatusMessage.text = "CONNECTION: " + status.ToString();
                _messagesStatusMessage.text = "MESSAGES: " + "Not Receiving";
                break;
            case BIOPACClient.Status.Connected:
                _connectionStatusImg.color = Color.green;
                _messagesStatusImg.color = Color.red;
                _connectionStatusMessage.text = "CONNECTION: " + "Connected";
                _messagesStatusMessage.text = "MESSAGES: " + "Not Receiving";
                break;
            case BIOPACClient.Status.Receving:
                _connectionStatusImg.color = Color.green;
                _messagesStatusImg.color = Color.green;
                _connectionStatusMessage.text = "CONNECTION: " + "Connected";
                _messagesStatusMessage.text = "MESSAGES: " + "Receiving";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }

    private void SetButtonsState(BIOPACClient.Status status)
    {
        _disconnectBtn.interactable = (status == BIOPACClient.Status.Connected) || (status == BIOPACClient.Status.Receving);
        _connectBtn.interactable = status == BIOPACClient.Status.Disconnected;
        
    }

    private void OnDestroy()
    {
        UnregisterUICallbacks();
        _biopacClient.ConnectionStatusChange -= OnConnectionStatusChange;  

    }

    private void UnregisterUICallbacks()
    {
        _disconnectBtn.onClick.RemoveAllListeners();
        _connectBtn.onClick.RemoveAllListeners();
        
        _ipIf.onEndEdit.RemoveAllListeners();
        _portIf.onEndEdit.RemoveAllListeners();
    }

    private void RegisterUICallbacks()
    {
        _disconnectBtn.onClick.AddListener(_biopacClient.Disconnect);
        _connectBtn.onClick.AddListener(_biopacClient.Connect);
        
        _ipIf.onEndEdit.AddListener((x =>
        {
            //Verify Correctness of IP Address
            _biopacClient.IPAddress = x;
        }));
        
        _portIf.onEndEdit.AddListener(x =>
        {
            //Verify Correctness of PORT Address
            int port = Int32.Parse(x);
            _biopacClient.Port = port;
        });
    }
}
