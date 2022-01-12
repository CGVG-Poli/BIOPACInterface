using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;

public class BIOPACSession
{
    public string ConnectedClient = "Not Connected";
    public double ConnectedClientClockDesync;
    public string AnalysisName;
    public string RespondentName;
    public DateTime RecordingSessionStart;
    public DateTime SlideshowStart;
    public DateTime SlideshowEnd;

    public bool IsSlideshowCompleted()
    {
        return SlideshowEnd != DateTime.MinValue;
    }
}
public class BIOPACSessionManager : MonoBehaviour
{
    private BIOPACSession _currentSession;

    private string _lastConnectedClient = "Not Connected";
    void Start()
    {
        BIOPACMessageHandler.Instance.SlideshowStarted += OnSlideshowStarted;
        BIOPACMessageHandler.Instance.SlideshowStopped += OnSlideshowStopped;
        BIOPACInterfaceServer.Instance.ClientConnected += message =>
        {
            _lastConnectedClient = message.ClientDeviceName;
        };
    }

    private void OnDestroy()
    {
        if(_currentSession == null)
            return;
        
        //Otherwise application is quitting but session has not been closed yet
        Slideshow slideshow = new Slideshow();
        slideshow.End = DateTime.MinValue;
        OnSlideshowStopped(slideshow);
    }

    private void OnSlideshowStarted(Slideshow slideshow)
    {
        _currentSession = new BIOPACSession();
        _currentSession.AnalysisName = slideshow.AnalysisName;
        _currentSession.RespondentName = slideshow.RespondentName;
        _currentSession.RecordingSessionStart = DateTime.Now;
        _currentSession.SlideshowStart = slideshow.Start;
        _currentSession.ConnectedClient = _lastConnectedClient;
        _currentSession.ConnectedClientClockDesync = SyncManager.Instance.LastComputedDelta;
        
        BIOPACSessionUI.Instance.SetSessionInfos(_currentSession);

        string filePath = Path.Combine(FileManager.Instance.SlideshowsOutputFolder, "biopac_sessions_summary.csv");
        WriteSessionInfoToFile(_currentSession, filePath);
    }

    private void OnSlideshowStopped(Slideshow slideshow)
    {
        _currentSession.SlideshowEnd = slideshow.End;        

        if(BIOPACSessionUI.Instance != null)
            BIOPACSessionUI.Instance.SetSessionInfos(_currentSession);
        
        
        //WRITE INFORMATIONS TO FILE
        
        string filePath = Path.Combine(FileManager.Instance.SlideshowsOutputFolder, "biopac_sessions_summary.csv");
        DeleteLastLine(filePath);
        WriteSessionInfoToFile(_currentSession, filePath);

        _currentSession = null;
    }

    private void WriteSessionInfoToFile(BIOPACSession session, string filePath)
    {
        if (!File.Exists(filePath))
            File.WriteAllText(filePath, "AnalysisName;RespondentName;RecordingSessionStart;SlideshowStart;SlideshowEnd;ConnectedClient;ConnectedClientClockDesync\n");

        StringBuilder sb = new StringBuilder();
        sb.Append(session.AnalysisName).Append(";");
        sb.Append(session.RespondentName).Append(";");
        sb.Append(session.RecordingSessionStart.ToString("yyyy/M/d HH:mm:ss.fff")).Append(";");
        sb.Append(session.SlideshowStart.ToString("yyyy/M/d HH:mm:ss.fff")).Append(";");
        string slideshowEnd = session.IsSlideshowCompleted() ? session.SlideshowEnd.ToString("yyyy/M/d HH:mm:ss.fff") : "Not Completed";
        sb.Append(slideshowEnd).Append(";");
        sb.Append(session.ConnectedClient).Append(";");
        sb.Append(session.ConnectedClientClockDesync).AppendLine();

        File.AppendAllText(filePath, sb.ToString());
    }

    private void DeleteLastLine(string filePath)
    {
        List<string> lines = File.ReadAllLines(filePath).ToList();
        if (lines.Count == 1) //File only contains Header
            return;

        File.WriteAllLines(filePath, lines.GetRange(0, lines.Count - 1).ToArray());
    }
    
}
