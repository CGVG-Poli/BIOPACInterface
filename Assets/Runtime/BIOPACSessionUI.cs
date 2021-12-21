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

    private bool _fileBrowserOpen = false;
    
    public string SlideshowOutputFolder { set
        {
            _slideshowFolderText.text = value;
        } }

    void Start()
    {
        //_selectSlideshowFolder.onClick.AddListener(() => FileManager.Instance.FileBrowserSelectSlideshowOutput());
        _selectSlideshowFolder.onClick.AddListener(() => StartCoroutine(OpenFileBrowser()));
        FileManager.Instance.FileBrowserClosed += () =>  _fileBrowserOpen = false;
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
