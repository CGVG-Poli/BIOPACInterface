using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;

public class FileManager : Singleton<FileManager>
{
    public event Action FileBrowserClosed;
    
    private string _slideshowsOutputFolder;
    public string SlideshowsOutputFolder { get { return _slideshowsOutputFolder; }
        set
        {
            _slideshowsOutputFolder = value;
            BIOPACSessionUI.Instance.SlideshowOutputFolder = _slideshowsOutputFolder;
        } 
    }

    protected override void Awake()
    {
        base.Awake();
        string lastSavedSlideshowOutputFolder = PlayerPrefs.GetString("LastSlideshowOutputFolder");
        if (string.IsNullOrEmpty(lastSavedSlideshowOutputFolder))
            SlideshowsOutputFolder = Path.Combine(Application.dataPath, "SlideshowsOutput");
        else
            SlideshowsOutputFolder = lastSavedSlideshowOutputFolder;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            FileBrowserSelectSlideshowOutput();
        }
    }

    public void FileBrowserSelectSlideshowOutput()
    {
        FileBrowser.ShowLoadDialog((paths) => 
        {
            SlideshowsOutputFolder = paths[0];
            PlayerPrefs.SetString("LastSlideshowOutputFolder", SlideshowsOutputFolder);
            Debug.Log($"Selected new Slideshow Output folder {SlideshowsOutputFolder}");
            FileBrowserClosed?.Invoke();
        },
            () =>
            {
                Debug.Log($"Canceled Slideshow Output folder selection, keeping: {SlideshowsOutputFolder}");
                FileBrowserClosed?.Invoke();
            },
        FileBrowser.PickMode.Folders, false, SlideshowsOutputFolder, null, "Select Slideshows Output Folder", "Select");
    }



}
