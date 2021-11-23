using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using System;

public class LogManager : MonoBehaviour
{
    [SerializeField] private string _fileName;
    private static LogManager _instance;
    private string _filePath = "";
    private string _fullPath = "";
    public static LogManager Instance => _instance;

    void Awake()
    {
        Assert.IsNull(_instance, "There is already an instance of LogManager in the scene");
        _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetFilePath(Application.dataPath);
    }   
   
    public void WriteLine(string line)
    {
        if ((string.IsNullOrEmpty(_fileName) && string.IsNullOrEmpty(_fullPath)) || string.IsNullOrEmpty(line))
            return;
        
        File.AppendAllText(_fullPath, line);
    }

    public void SetFilePath(string path)
    {
        /*
        _filePath = Path.Combine(path, "LogManager");
        if (!Directory.Exists(_filePath))
            Directory.CreateDirectory(_filePath);
        */

        _fullPath = Path.Combine(path, _fileName + ".txt");
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        
        _filePath = path;

    }

    public string FilePath
    {
        get
        {
            return _filePath;
        }
    }

    
}
