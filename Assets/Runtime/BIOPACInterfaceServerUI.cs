using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BIOPACInterfaceServerUI : Singleton<BIOPACInterfaceServerUI>
{
    [SerializeField] private InputField _portIF;
    [SerializeField] private Button _startServerBtn;
    
    [SerializeField] private RectTransform _connectedClientsRoot;
    [SerializeField] private GameObject _connectedClientUI;

    private GameObject _connectedClientUIGO;
    void Start()
    {
        _startServerBtn.onClick.AddListener((() =>
        {
            BIOPACInterfaceServer.Instance.StartServer(Int32.Parse(_portIF.text));
        }));

        BIOPACInterfaceServer.Instance.ServerStarted += () =>
        {
            _startServerBtn.GetComponentInChildren<Text>().text = "Stop Server";
            _startServerBtn.onClick.RemoveAllListeners();
            _startServerBtn.onClick.AddListener(() => BIOPACInterfaceServer.Instance.StopServer());
        };
        
        BIOPACInterfaceServer.Instance.ServerStopped += () =>
        {
            _startServerBtn.GetComponentInChildren<Text>().text = "Start Server";
            _startServerBtn.onClick.RemoveAllListeners();
            _startServerBtn.onClick.AddListener(() =>
                BIOPACInterfaceServer.Instance.StartServer(Int32.Parse(_portIF.text)));
        };
        
        BIOPACInterfaceServer.Instance.ClientConnected += OnClientConnected;
        BIOPACInterfaceServer.Instance.ClientDisconnected += OnClientDisconnected;

        
        _portIF.text = BIOPACInterfaceServer.Instance.DefaultConnectionPort.ToString();
        _portIF.textComponent.text = BIOPACInterfaceServer.Instance.DefaultConnectionPort.ToString();
    }

    private void OnClientDisconnected()
    {
        if(_connectedClientUIGO != null)
            Destroy(_connectedClientUIGO);

        _connectedClientUIGO = null;
    }

    private void OnClientConnected(ClientInformationMessage message)
    {
        _connectedClientUIGO = Instantiate(_connectedClientUI, _connectedClientsRoot);

        ConnectedClientUI connectedClientUI = _connectedClientUIGO.GetComponent<ConnectedClientUI>();
        connectedClientUI.ClientName.text = message.ClientDeviceName;
        
    }
}
