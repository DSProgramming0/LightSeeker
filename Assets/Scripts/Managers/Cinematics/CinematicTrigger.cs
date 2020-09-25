using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other) //Calling the cinematic events
    {
        GameEvents.instance.CinematicTriggerEnter();
    }

    void OnTriggerExit(Collider other)
    {
        GameEvents.instance.CinematicTriggerExit();
    }
}
