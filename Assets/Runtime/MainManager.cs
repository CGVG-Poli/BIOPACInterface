using System;
using System.Collections;
using System.Collections.Generic;
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
        ThreadManager.UpdateMain();        
    }
}
