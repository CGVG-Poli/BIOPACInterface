using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class BIOPACSession
{
    public string ConnectedClient;
    public double ConnectedClientClockDesync;
    public string AnalysisName;
    public string RespondentName;
    public DateTime RecordingSessionStart;
    public DateTime SlideshowStart;
    public DateTime SlideshowEnd;
}
public class BIOPACSessionManager : MonoBehaviour
{
    private BIOPACSession _currentSession;

    private string _lastConnectedClient;
    void Start()
    {
        BIOPACMessageHandler.Instance.SlideshowStarted += OnSlideshowStarted;
        BIOPACMessageHandler.Instance.SlideshowStopped += OnSlideshowStopped;
        BIOPACInterfaceServer.Instance.ClientConnected += message =>
        {
            _lastConnectedClient = message.ClientDeviceName;
        };
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
        
        StringBuilder sb = new StringBuilder();
        sb.Append(_currentSession.AnalysisName).AppendLine();
        sb.Append(_currentSession.RespondentName).AppendLine();
        sb.Append(_currentSession.RecordingSessionStart.ToString("yyyy/M/d HH:mm:ss.fff")).AppendLine();
        sb.Append(_currentSession.SlideshowStart.ToString("yyyy/M/d HH:mm:ss.fff")).AppendLine();
        //sb.Append(_currentSession.SlideshowEnd.ToString("yyyy/M/d HH:mm:ss.fff")).AppendLine();
        sb.Append(_currentSession.ConnectedClient).AppendLine();
        sb.Append(_currentSession.ConnectedClientClockDesync).AppendLine();
        BIOPACSessionUI.Instance.SessionStatus.text = sb.ToString();
    }

    private void OnSlideshowStopped(Slideshow slideshow)
    {
        _currentSession.SlideshowEnd = slideshow.End;

        // WRITE INFORMATION TO UI
        StringBuilder sb = new StringBuilder();
        sb.Append(_currentSession.AnalysisName).AppendLine();
        sb.Append(_currentSession.RespondentName).AppendLine();
        sb.Append(_currentSession.RecordingSessionStart.ToString("yyyy/M/d HH:mm:ss.fff")).AppendLine();
        sb.Append(_currentSession.SlideshowStart.ToString("yyyy/M/d HH:mm:ss.fff")).AppendLine();
        sb.Append(_currentSession.SlideshowEnd.ToString("yyyy/M/d HH:mm:ss.fff")).AppendLine();
        sb.Append(_currentSession.ConnectedClient).AppendLine();
        sb.Append(_currentSession.ConnectedClientClockDesync).AppendLine();
        BIOPACSessionUI.Instance.SessionStatus.text = sb.ToString();
        
        
        //WRITE INFORMATIONS TO FILE
        
        string filePath = Path.Combine(FileManager.Instance.SlideshowsOutputFolder, "biopac_sessions_summary.csv");

        if(!File.Exists(filePath))
            File.WriteAllText(filePath, "AnalysisName;RespondentName;RecordingSessionStart;SlideshowStart;SlideshowEnd;ConnectedClient;ConnectedClientClockDesync");
        
        sb = new StringBuilder();
        sb.Append(_currentSession.AnalysisName).Append(";");
        sb.Append(_currentSession.RespondentName).Append(";");
        sb.Append(_currentSession.RecordingSessionStart.ToString("yyyy/M/d HH:mm:ss.fff")).Append(";");
        sb.Append(_currentSession.SlideshowStart.ToString("yyyy/M/d HH:mm:ss.fff")).Append(";");
        sb.Append(_currentSession.SlideshowEnd.ToString("yyyy/M/d HH:mm:ss.fff")).Append(";");
        sb.Append(_currentSession.ConnectedClient).Append(";");
        sb.Append(_currentSession.ConnectedClientClockDesync).AppendLine();
        
        File.AppendAllText(filePath, sb.ToString());
    }
    
}
