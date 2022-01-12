using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BIOPACInterfaceClientUI : Singleton<BIOPACInterfaceClientUI>
{
    [SerializeField] private GameObject _mainUI;
    
    [SerializeField] private Button _startClientBtn;
    [SerializeField] private Button _stopClientBtn;

    [SerializeField] private Text _messagesText;
    
    void Start()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            
            GameObject eventSystemGo = new GameObject("Event System", typeof(EventSystem), 
                                                                                        typeof(StandaloneInputModule));
            eventSystemGo.transform.SetParent(this.transform);
        }
        
        _startClientBtn.onClick.AddListener(() =>
        {
            _startClientBtn.interactable = false;
            _stopClientBtn.interactable = true;
            
            BIOPACInterfaceClient.Instance.StartClient();

            _messagesText.text = "Looking for server...";
        });

        _stopClientBtn.interactable = false;
        _stopClientBtn.onClick.AddListener(() =>
        {
            _startClientBtn.interactable = true;
            _stopClientBtn.interactable = false;
            
            BIOPACInterfaceClient.Instance.StopClient();
            _messagesText.text = "";
        });

        BIOPACInterfaceClient.Instance.ConnectedToServer +=
            peer => _messagesText.text = "Connected to server @" + peer.EndPoint.Address;

        BIOPACInterfaceClient.Instance.DisconnectedFromServer += peer =>
        {
            _startClientBtn.interactable = true;
            _stopClientBtn.interactable = false;
            
            _messagesText.text = "Connected to server @" + peer.EndPoint.Address;
        };
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
            _mainUI.SetActive(!_mainUI.activeSelf);
    }
}
