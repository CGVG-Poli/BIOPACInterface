using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ClientServerSelector : MonoBehaviour
{
    private static ClientServerSelector instance;
    [SerializeField] UnityClient unityClient;
    [SerializeField] UnityServer unityServer;
    [SerializeField] ClientInterface clientInterface;
    [SerializeField] Text titleText, captionTextIP, captionTextPort;
    [SerializeField] GameObject consolePanel;
    [SerializeField] InputField inputFieldIP, inputFieldPort;
    [SerializeField] Button selectClientButton, selectServerButton, startButton;

    private enum ApplicationMode {
        None,
        Server,
        Client
    }

    private ApplicationMode applicationMode = ApplicationMode.None;

    private void Awake()
    {
        Assert.IsNull(instance);
        instance = this;
    }

    public static ClientServerSelector Instance
    {
        get
        {
            return instance;
        }
    }

    private void Start()
    {
        titleText.text = "PLEASE SELECT YOUR ROLE";

        selectClientButton.onClick.AddListener(ClientRoleSelected);
        selectServerButton.onClick.AddListener(ServerRoleSelected);
        startButton.onClick.AddListener(StartButtonPressed);
        consolePanel.SetActive(false);

        titleText.text = "Please select the Application Mode:";
        inputFieldIP.text = "127.0.0.1";
        inputFieldPort.text = "55010";

        ToggleSocketInformationInputFields(false);
    }

    private void ClientRoleSelected()
    {
        titleText.text = "CLIENT MODE";
        titleText.color = Color.blue;
        selectClientButton.gameObject.SetActive(false);
        selectServerButton.gameObject.SetActive(false);
        applicationMode = ApplicationMode.Client;
        ToggleSocketInformationInputFields(true);
        GeneralSetupInterface.Instance.gameObject.SetActive(false);
    }

    private void ServerRoleSelected()
    {
        titleText.text = "SERVER MODE";
        titleText.color = Color.red;
        selectClientButton.gameObject.SetActive(false);
        selectServerButton.gameObject.SetActive(false);
        applicationMode = ApplicationMode.Server;
        ToggleSocketInformationInputFields(true);
        GeneralSetupInterface.Instance.gameObject.SetActive(false);
    }

    private void StartButtonPressed()
    {

        switch (applicationMode)
        {
            case ApplicationMode.Client:
                unityClient.gameObject.SetActive(true);
                unityClient.enabled = true;
                unityClient.ipAddress = inputFieldIP.text;
                clientInterface.gameObject.SetActive(true);
                unityClient.port = int.Parse(inputFieldPort.text);
                ParserController.Instance.StartParsingOperations();
                break;
            case ApplicationMode.Server:
                unityServer.gameObject.SetActive(true);
                unityServer.enabled = true;
                unityServer.ipAddress = inputFieldIP.text;
                unityServer.port = int.Parse(inputFieldPort.text);
                ParserController.Instance.StartParsingOperations();
                break;
            default:
                Debug.LogError("ERROR: APPLICATION MODE IS NEITHER CLIENT NOR SERVER!!!");
                break;
        }

        consolePanel.gameObject.SetActive(true);

        Destroy(gameObject);
    }

    private void ToggleSocketInformationInputFields(bool activate)
    {
        startButton.gameObject.SetActive(activate);
        inputFieldIP.gameObject.SetActive(activate);
        inputFieldPort.gameObject.SetActive(activate);
        captionTextIP.gameObject.SetActive(activate);
        captionTextPort.gameObject.SetActive(activate);
    }
}
