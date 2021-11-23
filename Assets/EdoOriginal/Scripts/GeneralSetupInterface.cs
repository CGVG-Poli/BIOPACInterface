using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class GeneralSetupInterface : MonoBehaviour
{
    private static GeneralSetupInterface instance;
    [SerializeField] private Dropdown dropdownSeparator;
    [SerializeField] private Dropdown dropdownEncoding;
    [SerializeField] private Text captionTemplate, captionSource;
    [SerializeField] private Button buttonLoadTemplate, buttonLoadSource, buttonRevertTemplate, buttonRevertSource, parserTestButton;
    [SerializeField] private GameObject parserTestPanel;

    private void Awake()
    {
        Assert.IsNull(instance);
        instance = this;
    }

    public static GeneralSetupInterface Instance
    {
        get
        {
            return instance;
        }
    }

    void Start()
    {
        //DROPDOWN SETUP
        List<string> separatorOptions = new List<string>();
        for (int i = 0; i < (int)SeparatorType.Count; i++)
        {
            SeparatorType st = (SeparatorType)i;
            separatorOptions.Add(st.ToString());
        }

        dropdownSeparator.ClearOptions();
        dropdownSeparator.AddOptions(separatorOptions);
        dropdownSeparator.value = (int)ParserController.Instance.SeparatorType;

        List<string> encodingOptions = new List<string>();
        for (int i = 0; i < (int)EncodingType.Count; i++)
        {
            EncodingType et = (EncodingType)i;
            encodingOptions.Add(et.ToString());
        }

        dropdownEncoding.ClearOptions();
        dropdownEncoding.AddOptions(encodingOptions);
        dropdownEncoding.value = (int)EncodingController.Instance.EncodingType;

        //CAPTIONS SETUP
        captionSource.color = Color.green;
        captionSource.text = "(Using Default)";
        captionTemplate.color = Color.green;
        captionTemplate.text = "(Using Default)";

        //ADD BUTTON LISTENERS
        buttonLoadSource.onClick.AddListener(ButtonLoadSourcePressed);
        buttonLoadTemplate.onClick.AddListener(ButtonLoadTemplatePressed);
        buttonRevertSource.onClick.AddListener(ButtonRevertSourcePressed);
        buttonRevertTemplate.onClick.AddListener(ButtonRevertTemplatePressed);
        parserTestButton.onClick.AddListener(ParserTestButtonPressed);
    }

    public void SeparatorChanged()
    {
        int val = dropdownSeparator.value;
        ParserController.Instance.SeparatorType = (SeparatorType)val;
    }

    public void EncodingChanged()
    {
        int val = dropdownEncoding.value;
        EncodingController.Instance.EncodingType = (EncodingType)val;
    }

    private void ButtonLoadSourcePressed()
    {
        captionSource.color = Color.red;
        captionSource.text = "(Custom)";
    }

    private void ButtonLoadTemplatePressed()
    {
        captionTemplate.color = Color.red;
        captionTemplate.text = "(Custom)";
    }

    private void ButtonRevertSourcePressed()
    {
        captionSource.color = Color.green;
        captionSource.text = "(Using Default)";
    }

    private void ButtonRevertTemplatePressed()
    {
        captionTemplate.color = Color.green;
        captionTemplate.text = "(Using Default)";
    }

    private void ParserTestButtonPressed()
    {
        ParserController.Instance.StartParsingOperations();
        parserTestPanel.SetActive(true);
        ClientServerSelector.Instance.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
