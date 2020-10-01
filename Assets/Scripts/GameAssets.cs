using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(this.transform.parent.gameObject);
    }

    public static GameAssets instance
    {
        get
        {
            return _instance;
        }
    }

}

 