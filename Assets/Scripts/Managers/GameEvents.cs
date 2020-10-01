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

    public event Action onCinematicTriggerEnter; //2 suscribers on particle effect Controller & cinematic controller, called on cinematic trigger enter
    public void CinematicTriggerEnter()
    {
        if(onCinematicTriggerEnter != null)
        {
            onCinematicTriggerEnter();
        }
    }

    public event Action onCinematicTriggerExit; //2 suscribers on particle effect controller & cinematic controller, called when unpausing on cinematic controller and to reset particles on particle effect controller
    public void CinematicTriggerExit()
    {
        if (onCinematicTriggerExit != null)
        {
            onCinematicTriggerExit();
        }
    }

    public event Action onLightPillarActivated; //1 suscriver on gameManager, called on light pillar activation
    public void LightPillarActivated()
    {
        if (onLightPillarActivated != null)
        {
            onLightPillarActivated();
        }
    }

    public event Action onCompanionChanged; 
    public void CompanionTargetChanged()
    {
        if (onCompanionChanged != null)
        {
            onCompanionChanged();
        }
    }
}
