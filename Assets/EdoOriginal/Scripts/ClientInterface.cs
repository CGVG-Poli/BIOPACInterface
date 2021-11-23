using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ClientInterface : MonoBehaviour
{
    [SerializeField] private Button receiveEveryhtingButton, parseRelevantFieldsButton;
    [SerializeField] private UnityClient unityClient;
    [SerializeField] private Text modeCaption, titleText;
    private void Start()
    {
        modeCaption.gameObject.SetActive(true);
        receiveEveryhtingButton.onClick.AddListener(ReceiveEverythingButtonPressed);
        parseRelevantFieldsButton.onClick.AddListener(ParseRelevantButtonPressed);
    }

    //NEWSTUFF (MOD)
    private void ReceiveEverythingButtonPressed()
    {
        titleText.text = "CLIENT MODE (Receive Everything)";
        titleText.color = Color.blue;
        unityClient.SetMessageParsing(false);
        OpenConnection();
    }

    private void ParseRelevantButtonPressed()
    {
        titleText.text = "CLIENT MODE (Parse Relevant Fields)";
        titleText.color = Color.green;
        unityClient.SetMessageParsing(true);
        OpenConnection();
    }

    private void OpenConnection()
    {
        receiveEveryhtingButton.gameObject.SetActive(false);
        parseRelevantFieldsButton.gameObject.SetActive(false);
        modeCaption.gameObject.SetActive(false);
        unityClient.OpenConnection();
    }
}
