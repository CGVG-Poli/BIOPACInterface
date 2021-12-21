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
}
