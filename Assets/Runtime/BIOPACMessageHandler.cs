using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


//Example of Slideshow start
// 07042905;AttentionTool;SlideshowStart;0;-1;20211124112050433;Francesco_POLI;MALE;32;BIOPACInterface_v2;{"usingBlocks":false,"predefinedStudy":false,"stimuli":[{"id":1000,"file":"C:\\ProgramData\\iMotions\\Lab_NG\\Data\\BIOPACInterface_v2\\Stimuli\\VAS (4).png","type":"Image","format":"none","exposureTimeMs":300000.0,"name":"VAS (4)","locked":false}]}

//Example of Slideshow end
//07081788;AttentionTool;SlideshowEnd;19428.3951;-1;20211124112109861



public class BIOPACMessageHandler : Singleton<BIOPACMessageHandler>
{
    private string _outputDumpFilePath = "C:\\Users\\puniTO\\BIOPAC\\BIOPACOutputs\\biopacOutputDump.txt";

    private void Start()
    {
        if (!File.Exists(_outputDumpFilePath))
        {
            File.WriteAllText(_outputDumpFilePath, "Starting a new BIOPAC Connection", System.Text.Encoding.UTF8);
            return;
        }

        
    }

    public void MessageReceived(string msg)
    {
        File.AppendAllText(_outputDumpFilePath, msg);
    }
}
