using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;

public class FileManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            FileBrowser.ShowLoadDialog((paths) => { Debug.Log("Selected: " + paths[0]); },
        						   () => { Debug.Log( "Canceled" ); },
        						   FileBrowser.PickMode.Folders, false, null, null, "Select Folder", "Select" );
        }
    }

}
