using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class CinematicTrigger : MonoBehaviour
{
    [Header("Standard cutscene")]
    [SerializeField] private CinematicType thisCinematicType;
    [SerializeField] private CinemachineDollyCart thisDollyCart;
    [SerializeField] private CinemachineVirtualCamera thisVCam;
    [SerializeField] private PlayableDirector thisDirector;
    [SerializeField] private LightPillarActivation thisLightPillar;
    [SerializeField] private Transform thisPlayerResetPos;
    [SerializeField] private Transform thisPlayerLookAtPoint;
    [SerializeField] private bool hasPlayed;

    [Header("Intro cutscene")]
    [SerializeField] private GameObject cinematicPlayerModel;
    [SerializeField] private GameObject cinematicCompanionModel;

    void OnTriggerEnter(Collider other) //Calling the cinematic events
    {
        if(other.gameObject.tag == "Player")
        {
            if (!hasPlayed)
            {
                StartCoroutine(startingCinematic());
                hasPlayed = true;
            }
        }       
    }
   
    private IEnumerator startingCinematic() //Sets the cinematicController components by passing this triggers local variables
    {
        if(thisCinematicType == CinematicType.PLAYERDRIVEN)
        {
            CinematicController.instance.setCurrentPlayerDrivenCinematicComponents(thisCinematicType, thisDollyCart, thisVCam, thisDirector, thisLightPillar, thisPlayerResetPos, thisPlayerLookAtPoint);
        }
        else if (thisCinematicType == CinematicType.STANDARD)
        {
            CinematicController.instance.setCurrentStandardCinematicComponenets(thisCinematicType, thisDirector, cinematicPlayerModel, cinematicCompanionModel);
        }

        yield return new WaitForSeconds(.25f);

        GameEvents.instance.CinematicTriggerEnter(); //Calls the event 

        StopCoroutine(startingCinematic());
    }
}
