using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOL : MonoBehaviour
{
    public static DontDestroyOL Singleton;
    
    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        
        DontDestroyOnLoad(gameObject);
    }
}
