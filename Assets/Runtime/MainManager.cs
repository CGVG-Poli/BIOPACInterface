using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : Singleton<MainManager>
{
    
    
    void Update()
    {
        ThreadManager.UpdateMain();        
    }
}
