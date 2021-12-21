using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ConsoleDebugger : Singleton<ConsoleDebugger>
{
    [SerializeField] private Text _text;
    [SerializeField] private Button _showButton;
    [SerializeField] private Button _clearButton;
    [SerializeField] private GameObject _consoleContainer;
    
    private void Start()
    {
        if (_text == null)
        {
            Debug.LogWarning($"Missing Text component in ConsoleDebugger, finding one in childrens");
            _text = GetComponentInChildren<Text>();
        }
        _text.text = "";
        
        _showButton.onClick.AddListener(() => _consoleContainer.SetActive(!_consoleContainer.activeSelf));
        _clearButton.onClick.AddListener(() =>_text.text = "");
    }

    public void Log(string message)
    {
        Debug.Log(message);
        if(_text == null)
            return;
        
        _text.text += message + "\n";
    }
}
