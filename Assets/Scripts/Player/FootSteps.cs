using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private PlayerManager playerManager;

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    private void Step()
    {
        if(playerManager.getMovementState() == true && playerManager.getWorldState() == PlayerWorldState.FREECONTROL)
        {
            soundManager.Step(false);
        }
    }

    private void Land()
    {
        soundManager.Step(true);
    }
}
