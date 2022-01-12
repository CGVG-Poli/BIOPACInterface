using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class MainManager : Singleton<MainManager>
{
    protected override void Awake()
    {
        base.Awake();
        Application.runInBackground = true;
    }
    void Update()
    {
        return;
        //DEBUG FUNCTIONS
        if (Input.GetKeyDown(KeyCode.C))
        {
            BIOPACInterfaceClient.Instance.StartClient();
        }


    }
}
