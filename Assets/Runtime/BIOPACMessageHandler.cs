using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.UI;
using System.Globalization;

//Example of Slideshow start
// 07042905;AttentionTool;SlideshowStart;0;-1;20211124112050433;Francesco_POLI;MALE;32;BIOPACInterface_v2;{"usingBlocks":false,"predefinedStudy":false,"stimuli":[{"id":1000,"file":"C:\\ProgramData\\iMotions\\Lab_NG\\Data\\BIOPACInterface_v2\\Stimuli\\VAS (4).png","type":"Image","format":"none","exposureTimeMs":300000.0,"name":"VAS (4)","locked":false}]}

//Example of Slideshow end
//07081788;AttentionTool;SlideshowEnd;19428.3951;-1;20211124112109861

public class Slideshow
{
    public DateTime Start;
    public string AnalysisName;
    public string RespondentName;
    public bool isRunning;

    public Slideshow(string biopacMessage)
    {
        string[] messageParts = biopacMessage.Split(';');
        Start = DateTime.ParseExact(messageParts[5], "yyyyMMddhhmmssfff", CultureInfo.InvariantCulture);
        RespondentName = messageParts[6];
        AnalysisName = messageParts[9];
        isRunning = true;
    }
}


public class BIOPACMessageHandler : Singleton<BIOPACMessageHandler>
{

    //DEBUG
    public Text messageReceived;
    public Text messageToProcess;

    public event Action<Slideshow> SlideshowStarted;
    public event Action<Slideshow> SlideshowStopped;
    
    private string _outputDumpFilePath = "C:\\Users\\puniTO\\BIOPAC\\BIOPACOutputs\\biopacOutputDump.txt";
    private string _outputSlideShowFilePath = "C:\\Users\\puniTO\\BIOPAC\\BIOPACOutputs\\biopacOutputSlideshow.txt";
    private string _debugOutputFilePath = "C:\\Users\\puniTO\\BIOPAC\\BIOPACOutputs\\debugOutput.txt";

    //private Queue<string> _receivedMessages;
    private BlockingCollection<string> _receivedMessages;
    private Slideshow _currentSlideshow;

    private int _messagesReceived = 0;
    private void Start()
    {
        File.WriteAllText(_outputDumpFilePath, "Starting a new BIOPAC Connection\n", System.Text.Encoding.UTF8);
        File.WriteAllText(_debugOutputFilePath, "Starting a new BIOPAC Connection\n", System.Text.Encoding.UTF8);
        
        _receivedMessages = new BlockingCollection<string>();
        Thread processMessageThread = new Thread(ProcessIncomingMessages);
        processMessageThread.Start();
    }

    private void Update()
    {
        //messageReceived.text = _messagesReceived.ToString();
        //messageToProcess.text = _receivedMessages.Count.ToString();

        if (Input.GetKeyDown(KeyCode.P))
        {
            Thread processMessageThread = new Thread(ProcessIncomingMessages);
            processMessageThread.Start();
        }
           
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 150, 15, 150, 20), "RECEIVED:" + _messagesReceived.ToString());
        GUI.Label(new Rect(Screen.width - 150, 15 + 20, 150, 20), "TO PROCESS:" + _receivedMessages.Count.ToString());
    }

    private void ProcessIncomingMessages()
    {
        while (true)
        {
            if (_receivedMessages.Count == 0)
                continue;

            string msg = _receivedMessages.Take();

            BIOPACMessages.Type messageType = GetMessageType(msg);
            if (messageType == BIOPACMessages.Type.Undefined)
                continue;

            if (messageType == BIOPACMessages.Type.SlideshowStart)
            {
                //The slideshow started
                _currentSlideshow = new Slideshow(msg);
                File.WriteAllText(_outputSlideShowFilePath, msg);
                ThreadManager.ExecuteOnMainThread(() =>
                    ConsoleDebugger.Instance.Log(
                        $"Slideshow Started. Analysis:{_currentSlideshow.AnalysisName}, Respondent:{_currentSlideshow.RespondentName}"));

                //DEBUG
                TimeSpan desync = DateTime.Now - _currentSlideshow.Start;
                ThreadManager.ExecuteOnMainThread(() =>
                    ConsoleDebugger.Instance.Log(
                        $"Desync between clocks:{desync.ToString("G")}"));


                SlideshowStarted?.Invoke(_currentSlideshow);
                continue;
            }

            if (messageType == BIOPACMessages.Type.SlideshowEnd && _currentSlideshow != null)
            {
                //The slideshow has ended
                ThreadManager.ExecuteOnMainThread(() =>
                    ConsoleDebugger.Instance.Log(
                        $"Slideshow Stopped. Analysis:{_currentSlideshow.AnalysisName}, Respondent:{_currentSlideshow.RespondentName}"));
                File.AppendAllText(_outputSlideShowFilePath, msg);
                _currentSlideshow.isRunning = false;
                SlideshowStopped?.Invoke(_currentSlideshow);
                
            }

            if (_currentSlideshow != null && _currentSlideshow.isRunning)
            {
                File.AppendAllText(_outputSlideShowFilePath, msg);
            }
        }
    }

    private BIOPACMessages.Type GetMessageType(string msg)
    {
        string[] messageParts = msg.Split(';');
        if (messageParts.Length < 2)
            return BIOPACMessages.Type.Undefined;

        BIOPACMessages.Type result;
        bool success = Enum.TryParse(messageParts[2], out result);
        if (success)
            return result;
        else
            return BIOPACMessages.Type.Undefined;
    }

    public void MessageReceived(string msg)
    {
        string[] messageEntries = msg.Split('\n');
        _messagesReceived += messageEntries.Length;
        for(int i = 0; i < messageEntries.Length; i++)
            _receivedMessages.Add(messageEntries[i]);
        
        File.AppendAllText(_outputDumpFilePath, msg);
    }
    
    
}
