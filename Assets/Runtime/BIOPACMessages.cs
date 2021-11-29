using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIOPACMessages
{
    public enum Type
    {
        Undefined,
        SlideshowStart,
        SlideshowEnd,
        EDA,
        ECG,
        RSP,
        Biopac
    }
}

//TODO define how to use these classes
public struct BIOPACMessage
{
    public bool Start;
    public string TimeStamp;
    public string AnalysisName;
    public string RespondentName;
}
