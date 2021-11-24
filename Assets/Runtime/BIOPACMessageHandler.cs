using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


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
    public event Action<Slideshow> SlideshowStarted;
    public event Action<Slideshow> SlideshowStopped;
    
    private string _outputDumpFilePath = "C:\\Users\\puniTO\\BIOPAC\\BIOPACOutputs\\biopacOutputDump.txt";

    private Queue<string> _receivedMessages;
    private Slideshow _currentSlideshow;
    private void Start()
    {
        if (!File.Exists(_outputDumpFilePath))
        {
            File.WriteAllText(_outputDumpFilePath, "Starting a new BIOPAC Connection", System.Text.Encoding.UTF8);
            return;
        }

        _receivedMessages = new Queue<string>();
    }

    private void Update()
    {
        if(_receivedMessages.Count == 0)
            return;

        string msg = _receivedMessages.Dequeue();
        Slideshow slideshow;
        bool isSlideshowEvent = false;
        
        if (_currentSlideshow == null)
        {
            isSlideshowEvent = IsSlideshowEvent(msg, out slideshow);

            if (isSlideshowEvent && slideshow.Start)
            {
                //Start logging to file
                _currentSlideshow = slideshow;
                File.WriteAllText(_outputDumpFilePath, msg);
                ConsoleDebugger.Instance.Log($"Slideshow Started. Analysis:{_currentSlideshow.AnalysisName}, Respondent:{_currentSlideshow.RespondentName}");
                SlideshowStarted?.Invoke(_currentSlideshow);
                return;
            }
            
            return;
        }
        
        isSlideshowEvent = IsSlideshowEvent(msg, out slideshow);

        if (!isSlideshowEvent)
        {
            File.AppendAllText(_outputDumpFilePath, msg);
            return;
        }

        if (isSlideshowEvent && !slideshow.Start)
        {
            ConsoleDebugger.Instance.Log($"Slideshow Stopped. Analysis:{_currentSlideshow.AnalysisName}, Respondent:{_currentSlideshow.RespondentName}");
            _currentSlideshow = null;
            File.AppendAllText(_outputDumpFilePath, msg);
            SlideshowStopped?.Invoke(_currentSlideshow);
            return;
        }

    }

    private bool IsSlideshowEvent(string msg, out Slideshow slideshow)
    {
        slideshow = new Slideshow();
        string[] messageParts = msg.Split(';');
        string slideshowEvent = messageParts[2];

        if (!slideshowEvent.Equals("SlideshowStart") || !slideshowEvent.Equals("SlideshowEnd"))
            return false;
        
        slideshow.Start = slideshowEvent.Equals("SlideshowStart");
        slideshow.TimeStamp = messageParts[5];

        if (slideshow.Start)
        {
            slideshow.RespondentName = messageParts[6];
            slideshow.AnalysisName = messageParts[9];
        }
        
        return true;
    }

    public void MessageReceived(string msg)
    {
        string[] messageEntries = msg.Split('\n');
        for(int i = 0; i < messageEntries.Length; i++)
            _receivedMessages.Enqueue(messageEntries[i]);
        
        File.AppendAllText(_outputDumpFilePath, msg);
    }
    
    
}
