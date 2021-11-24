using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.UI;


//Example of Slideshow start
// 07042905;AttentionTool;SlideshowStart;0;-1;20211124112050433;Francesco_POLI;MALE;32;BIOPACInterface_v2;{"usingBlocks":false,"predefinedStudy":false,"stimuli":[{"id":1000,"file":"C:\\ProgramData\\iMotions\\Lab_NG\\Data\\BIOPACInterface_v2\\Stimuli\\VAS (4).png","type":"Image","format":"none","exposureTimeMs":300000.0,"name":"VAS (4)","locked":false}]}

//Example of Slideshow end
//07081788;AttentionTool;SlideshowEnd;19428.3951;-1;20211124112109861

public class Slideshow
{
    public bool Start;
    public string TimeStamp;
    public string AnalysisName;
    public string RespondentName;
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
            Slideshow slideshow;
            bool isSlideshowEvent = false;

            if (_currentSlideshow == null)
            {
                isSlideshowEvent = IsSlideshowEvent(msg, out slideshow);

                if (isSlideshowEvent && slideshow.Start)
                {
                    //Start logging to file
                    _currentSlideshow = slideshow;
                    File.WriteAllText(_outputSlideShowFilePath, msg);
                    ThreadManager.ExecuteOnMainThread(() => ConsoleDebugger.Instance.Log($"Slideshow Started. Analysis:{_currentSlideshow.AnalysisName}, Respondent:{_currentSlideshow.RespondentName}"));
                    SlideshowStarted?.Invoke(_currentSlideshow);
                    continue;
                }

                continue;
            }

            isSlideshowEvent = IsSlideshowEvent(msg, out slideshow);

            if (!isSlideshowEvent)
            {
                File.AppendAllText(_outputSlideShowFilePath, msg);
                continue;
            }

            if (isSlideshowEvent && !slideshow.Start)
            {

                ThreadManager.ExecuteOnMainThread(() => ConsoleDebugger.Instance.Log($"Slideshow Stopped. Analysis:{_currentSlideshow.AnalysisName}, Respondent:{_currentSlideshow.RespondentName}"));
                _currentSlideshow = null;
                File.AppendAllText(_outputSlideShowFilePath, msg);
                SlideshowStopped?.Invoke(_currentSlideshow);
                continue;
            }
        }
        
    }

    private bool IsSlideshowEvent(string msg, out Slideshow slideshow)
    {
        slideshow = new Slideshow();
        string[] messageParts = msg.Split(';');

        if (messageParts.Length <= 7)
            return false;

        string slideshowEvent = messageParts[2];
        //File.AppendAllText(_debugOutputFilePath, slideshowEvent + "\n");
        //ThreadManager.ExecuteOnMainThread(() => Debug.Log());
        if (slideshowEvent.Equals("SlideshowStart") || slideshowEvent.Equals("SlideshowEnd"))
        {
            slideshow.Start = slideshowEvent.Equals("SlideshowStart");

            string text = slideshow.Start ? "Slide show started event" : "Slide show ended event";
            File.AppendAllText(_debugOutputFilePath, text);


            slideshow.TimeStamp = messageParts[5];

            if (slideshow.Start)
            {
                slideshow.RespondentName = messageParts[6];
                slideshow.AnalysisName = messageParts[9];
            }

            return true;

        }

        return false;
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
