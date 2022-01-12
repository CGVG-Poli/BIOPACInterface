using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BIOPACSessionUI : Singleton<BIOPACSessionUI>
{
    [SerializeField] private Text _slideshowFolderText;
    [SerializeField] private Button _selectSlideshowFolder;
    [SerializeField] private Button _openSlideshowFolder;
    //[SerializeField] private Text _sessionStatus;

    [SerializeField] private Text _analysisName;
    [SerializeField] private Text _respondentName;
    [SerializeField] private Text _recordingStart;
    [SerializeField] private Text _slideshowStart;
    [SerializeField] private Text _slideshowStop;
    [SerializeField] private Text _clientName;
    [SerializeField] private Text _clientDesync;

    private bool _fileBrowserOpen = false;
    
    public string SlideshowOutputFolder { set
        {
            _slideshowFolderText.text = value;
        } }

    void Start()
    {
        _selectSlideshowFolder.onClick.AddListener(() => StartCoroutine(OpenFileBrowser()));
        FileManager.Instance.FileBrowserClosed += () =>  _fileBrowserOpen = false;
        
        _openSlideshowFolder.onClick.AddListener(() =>
        {
            Application.OpenURL($"file://{FileManager.Instance.SlideshowsOutputFolder}");
        });

        _analysisName.text = "Not Started";
        _respondentName.text = "Not Started";
        _recordingStart.text = "Not Started";
        _slideshowStart.text = "Not Started";
        _slideshowStop.text = "Not Started";
        _clientName.text = "Not Started";
        _clientDesync.text = "Not Started";

    }

    private IEnumerator OpenFileBrowser()
    {
        if (_fileBrowserOpen)
            yield break;
        
        _fileBrowserOpen = true;
        FileManager.Instance.FileBrowserSelectSlideshowOutput();
        
        yield return new WaitForEndOfFrame();
        
        // This is a hack because the windows instantiates an event system which duplicates the one already present in 
        //the scene, raising a constant warning
        GameObject eventSystem = FindObjectOfType<FileBrowser>().GetComponentInChildren<EventSystem>()?.gameObject;
        if(eventSystem != null)
            Destroy(eventSystem);
    }

    public void SetSessionInfos(BIOPACSession session)
    {
        _analysisName.text = session.AnalysisName;
        _respondentName.text = session.RespondentName;
        _recordingStart.text = session.RecordingSessionStart.ToString("yyyy/M/d HH:mm:ss.fff");
        _slideshowStart.text = session.SlideshowStart.ToString("yyyy/M/d HH:mm:ss.fff");
        _slideshowStop.text = session.SlideshowEnd.ToString("yyyy/M/d HH:mm:ss.fff");
        _clientName.text = session.ConnectedClient;
        _clientDesync.text = session.ConnectedClientClockDesync.ToString();
    }
}
