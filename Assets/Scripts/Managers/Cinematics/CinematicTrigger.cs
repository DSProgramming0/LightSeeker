using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class CinematicTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart thisDollyCart;
    [SerializeField] private CinemachineVirtualCamera thisVCam;
    [SerializeField] private PlayableDirector thisDirector;

    [SerializeField] private bool hasPlayed;

    void OnTriggerEnter(Collider other) //Calling the cinematic events
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("PlayerDetected");
            if (!hasPlayed)
            {
                StartCoroutine(startingCinematic());
                hasPlayed = true;
            }
        }       
    }
   
    private IEnumerator startingCinematic() //Sets the cinematicController components by passing this triggers local variables
    {
        CinematicController.instance.setCurrentCinematicComponents(thisDollyCart, thisVCam, thisDirector);

        yield return new WaitForSeconds(.25f);

        GameEvents.instance.CinematicTriggerEnter(); //Calls the event 

        StopCoroutine(startingCinematic());
    }
}
