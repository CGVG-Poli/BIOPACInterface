using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BIOPACSessionUI : Singleton<BIOPACSessionUI>
{
    [SerializeField] private Text _slideshowFolderText;
    [SerializeField] private Button _selectSlideshowFolder;
    
    public string SlideshowOutputFolder { set
        {
            _slideshowFolderText.text = value;
        } }

    void Start()
    {
        _selectSlideshowFolder.onClick.AddListener(() => FileManager.Instance.FileBrowserSelectSlideshowOutput());
        
    }

    
}
