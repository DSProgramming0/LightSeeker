using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [SerializeField] private CinemachineFreeLook MainvCam;

    [SerializeField] private GameObject playerBody;

    public PlayerMovementStates playerMovementState;
    public PlayerInteractState playerInteractState;
    public PlayerWorldState playerWorldState;

    private PlayerMovement playerMovement;
    private PlayerInteract playerInteraction;
    private AnimatorHook animHook;

    void Awake()
    {
        playerMovementState = PlayerMovementStates.IDLE;
        playerInteractState = PlayerInteractState.NOTFOCUSING;
        playerWorldState = PlayerWorldState.FREECONTROL;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;       

        playerMovement = GetComponent<PlayerMovement>();
        playerInteraction = GetComponent<PlayerInteract>();
        animHook = GetComponent<AnimatorHook>();
    }   

    public void setMovementState(PlayerMovementStates _state)
    {
        playerMovementState = _state;
    }

    public void setInteractState(PlayerInteractState _state)
    {
        playerInteractState = _state;
    }

    public void setWorldState(PlayerWorldState _state)
    {
        playerWorldState = _state;
    }

    public PlayerMovementStates getCurrentMovmementState()
    {
        return playerMovementState;
    }

    public PlayerInteractState getInteractState()
    {
        return playerInteractState;
    }

    public PlayerWorldState getWorldState()
    {
        return playerWorldState;
    }   

    public void pausePlayer(bool _shouldPause, bool _shouldHide)
    {
        if (_shouldPause)
        {
            playerMovement.stopMovement(true);
            playerWorldState = PlayerWorldState.PLAYERPAUSED;
            MainvCam.m_XAxis.m_InputAxisName = "";
            MainvCam.m_YAxis.m_InputAxisName = "";
            MainvCam.m_YAxis.m_InputAxisValue = 0;
            MainvCam.m_XAxis.m_InputAxisValue = 0;


            if (_shouldHide)
            {
                playerBody.SetActive(false);
            }
        }
        else
        {
            playerMovement.stopMovement(false);
            playerWorldState = PlayerWorldState.FREECONTROL;
            MainvCam.m_XAxis.m_InputAxisName = "Mouse X";
            MainvCam.m_YAxis.m_InputAxisName = "Mouse Y";
            playerBody.SetActive(true);
        }
    }
}

public enum PlayerMovementStates
{
    IDLE,
    WALKING,
    RUNNING,
    INCINEMATIC
}

public enum PlayerInteractState
{
    FOCUSING,
    NOTFOCUSING,
    INCINEMATIC
}

public enum PlayerWorldState
{
    PLAYERPAUSED,
    INCINEMATIC,
    FREECONTROL
}
