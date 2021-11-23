using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ClientServerSelectorSimple : MonoBehaviour
{
    private static ClientServerSelectorSimple instance;
    [SerializeField] UnityClient unityClient;
    //[SerializeField] ClientInterface clientInterface;
    [SerializeField] Text titleText, captionTextIP, captionTextPort;
    [SerializeField] GameObject consolePanel;
    [SerializeField] InputField inputFieldIP, inputFieldPort;
    [SerializeField] Button startButton;

    private void Awake()
    {
        Assert.IsNull(instance);
        instance = this;
    }

    public static ClientServerSelectorSimple Instance
    {
        get
        {
            return instance;
        }
    }

    private void Start()
    {
        titleText.text = "BIOPAC CLIENT";

        titleText.color = Color.blue;

        startButton.onClick.AddListener(StartButtonPressed);
        consolePanel.SetActive(false);

        inputFieldIP.text = "127.0.0.1";
        inputFieldPort.text = "55010";

        ToggleSocketInformationInputFields(true);
    }

    private void ToggleSocketInformationInputFields(bool activate)
    {
        startButton.gameObject.SetActive(activate);
        inputFieldIP.gameObject.SetActive(activate);
        inputFieldPort.gameObject.SetActive(activate);
        captionTextIP.gameObject.SetActive(activate);
        captionTextPort.gameObject.SetActive(activate);
    }

    private void StartButtonPressed()
    {
        unityClient.gameObject.SetActive(true);
        unityClient.enabled = true;
        unityClient.ipAddress = inputFieldIP.text;
        //clientInterface.gameObject.SetActive(true);
        unityClient.port = int.Parse(inputFieldPort.text);
        LogSetupPanel.Instance.gameObject.SetActive(false);
        //ParserController.Instance.StartParsingOperations();

        consolePanel.gameObject.SetActive(true);

        titleText.text = "BIOPAC CLIENT";
        titleText.color = Color.blue;
        unityClient.SetMessageParsing(false);
        OpenConnection();

        Destroy(gameObject);
    }



    private void OpenConnection()
    {
        unityClient.OpenConnection();
    }
}
