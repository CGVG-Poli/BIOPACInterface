using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
//using SFB; //LIBRARY IS NOT COMPILING

public class LogSetupPanel : MonoBehaviour
{
    private static LogSetupPanel instance;
    [SerializeField] private Button selectFolderButton;
    [SerializeField] private Text pathText;

    private void Awake()
    {
        Assert.IsNull(instance);
        instance = this;
    }

    public static LogSetupPanel Instance
    {
        get
        {
            return instance;
        }
    }

    private void Start()
    {
        UpdatePathText();
        selectFolderButton.onClick.AddListener(SelectFolderButtonPressed);
    }

    public void UpdatePathText()
    {
        pathText.text = LogManager.Instance.FilePath;
    }

    private void SelectFolderButtonPressed()
    {
        /* LIBRARY IS NOT COMPILING
        var extensions = new[] {
            new ExtensionFilter("directory", "" ),
        };

        //string selectedPath = StandaloneFileBrowser.SaveFilePanel("Select output directory", LogManager.Instance.FilePath, "log", extensions);
        var selectedPath = StandaloneFileBrowser.OpenFolderPanel("Select output directory", LogManager.Instance.FilePath, false);

        Debug.Log(selectedPath[0].ToString());

        if (selectedPath[0] != null)
        {
            LogManager.Instance.SetFilePath(selectedPath[0].ToString());
            UpdatePathText();
        }
        else
        {
            Debug.LogError("ERROR: Invalid folder!");
        }
        */
    }
}
