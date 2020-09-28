using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public event Action onCinematicTriggerEnter; //Called on the cinematic trigger class 
    public void CinematicTriggerEnter()
    {
        if(onCinematicTriggerEnter != null)
        {
            onCinematicTriggerEnter();
        }
    }

    public event Action onCinematicTriggerExit;
    public void CinematicTriggerExit()
    {
        if (onCinematicTriggerExit != null)
        {
            onCinematicTriggerExit();
        }
    }
}
