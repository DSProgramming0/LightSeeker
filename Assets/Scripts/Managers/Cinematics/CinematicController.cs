using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class CinematicController : MonoBehaviour
{
    public static CinematicController instance;

    [SerializeField] private AnimatorHook animHook;
    [SerializeField] private Transform player;
    [SerializeField] private CinemachineFreeLook playerMainCam;

    [Header("Standard Cinemachine")]
    [SerializeField] private CinematicType currentCinematicType;
    [SerializeField] private CinemachineDollyCart currentDollyCart;
    [SerializeField] private CinemachineVirtualCamera currentCinematicCam;
    [SerializeField] private PlayableDirector currentCinematicDirector;
    [SerializeField] private LightPillarActivation currentLightPillar;
    [SerializeField] private Transform currentPlayerResetPos;
    [SerializeField] private Transform lookAtTarget;
    [SerializeField] private float cinematicMovementSpeed;
    [SerializeField] private float timeIntoAnimation;
    [SerializeField] private  float interactHoldDownTime;
    [SerializeField] private bool cinematicMovementStopped;
    [SerializeField] private bool interactKeyPressRequired;
    private bool interactKeyPressSet = false;
    private bool lightBeamActivated = false;
    private bool musicChangeCalled = false;

    [Header("Intro Cinemachine")]
    [SerializeField] private GameObject currentCinematicPlayerModel;
    [SerializeField] private GameObject currentCinematicCompanionModel;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.instance.onCinematicTriggerEnter += pausePlayerCinematic;  //suscribing 2 methods to 2 game events
        GameEvents.instance.onCinematicTriggerExit += unPausePlayerCinematic;

        player = FindObjectOfType<PlayerMovement>().transform;
        animHook = player.GetComponent<AnimatorHook>();
    }

    #region componentSetters
    public void setCurrentPlayerDrivenCinematicComponents(CinematicType _type, CinemachineDollyCart _dollyCart, CinemachineVirtualCamera _vCam, PlayableDirector _director, LightPillarActivation _lightPillar, Transform _playerResetPos, Transform _lookAtPos)
    { //Sets cinematicControllers components dependent on the current cinematicTrigger entered
        currentCinematicType = _type;
        currentDollyCart = _dollyCart;
        currentCinematicCam = _vCam;
        currentCinematicDirector = _director;
        currentLightPillar = _lightPillar;
        currentPlayerResetPos = _playerResetPos;
        lookAtTarget = _lookAtPos;
    }

    public void setCurrentStandardCinematicComponenets(CinematicType _type, PlayableDirector _director, GameObject _playerModel, GameObject _companionModel)
    {
        currentCinematicType = _type;
        currentCinematicDirector = _director;
        currentCinematicPlayerModel = _playerModel;
        currentCinematicCompanionModel = _companionModel;
    }
    #endregion

    void Update()
    {
        if (PlayerManager.instance.getWorldState() == PlayerWorldState.INCINEMATIC)
        {
            playerMainCam.Priority = 7;
            if(currentCinematicType == CinematicType.PLAYERDRIVEN)
            {
                if (Input.GetKey(KeyCode.W) && !cinematicMovementStopped) //Moves Dolly cart and camera depending on w input if cinematicMovement is enabled
                {
                    timeIntoAnimation += cinematicMovementSpeed * Time.deltaTime; //W input increase float value which is given as a value to the cart postion and the camera time value.
                    currentDollyCart.m_Position = timeIntoAnimation;
                    currentCinematicDirector.time = timeIntoAnimation;
                    animHook.setSpeed(2f);
                }
                else
                {
                    currentCinematicDirector.time = timeIntoAnimation;
                    animHook.setSpeed(0f);
                }

                if (currentCinematicDirector.time >= currentCinematicDirector.duration - 1f) //Starts playing beam of light effect before end of animation
                {
                    StartCoroutine(playFreeTheLightEffect());
                }

                if (Input.GetKey(KeyCode.E))
                {
                    if (interactKeyPressRequired)
                    {
                        interactHoldDownTime += Time.deltaTime;
                        animHook.setInteractBool(true);

                        if (interactHoldDownTime >= 3f)
                        {
                            //PLAYER CAN LET GO OF INTERACT KEY HERE
                            interactKeyPressRequired = false; //Interact Animation still plays 
                            lightBeamActivated = true;
                        }
                    }
                }
                else if (lightBeamActivated)
                {
                    interactHoldDownTime = 0;
                    animHook.setInteractBool(true);
                }
                else
                {
                    interactHoldDownTime = 0; //If player lets go of interact before complete
                    animHook.setInteractBool(false);
                }
            }
            else if (currentCinematicType == CinematicType.STANDARD)
            {               
                if (currentCinematicDirector.time >= currentCinematicDirector.duration - 2f)
                {
                    Debug.Log("ending animation");
                    GameEvents.instance.CinematicTriggerExit();
                }
            }
        }       
    }

    public void pausePlayerCinematic() //Pauses player controls, sets them as a child of the card and switched camera //THIS STARTS CINEMATIC SEQUENCE
    {
        if (currentCinematicType == CinematicType.STANDARD)
        {
            PlayerManager.instance.pausePlayer(true, true);
        }
        else
        {
            PlayerManager.instance.pausePlayer(true, false);
        }

        PlayerManager.instance.setWorldState(PlayerWorldState.INCINEMATIC);

        UIManager.instance.startFade(.75f, false); //fade to black

        if(currentCinematicType == CinematicType.PLAYERDRIVEN)
        {
            StartCoroutine(setCinematicPosition());
        }
        else if(currentCinematicType == CinematicType.STANDARD)
        {
            StartCoroutine(toggleCinematicModels(true,.5f));
            StartCoroutine(playStandardCinematic());

        }
    }     

    public void unPausePlayerCinematic() //Resetting values of the cinematic Controller and giving the player control
    {
        PlayerManager.instance.setWorldState(PlayerWorldState.FREECONTROL);

        Debug.Log("Unpausing");
        player.transform.parent = null;

        if(currentCinematicType == CinematicType.PLAYERDRIVEN)
        {
            StartCoroutine(resetPlayerDrivenCinematicComponenets());
        }
        else if(currentCinematicType == CinematicType.STANDARD)
        {
            StartCoroutine(resetStandardCinematicComponents());
        }
    }

    #region PLAYER DRIVEN CINEMATICS

    private IEnumerator setCinematicPosition() //Delays palyer movement until blackout screen has unfaded and player pos is set in the cinematic dolly cart
    {
        cinematicMovementStopped = true;
        yield return new WaitForSeconds(.2f);
        currentCinematicCam.Priority = 15; //Switches camera to cinematic camera

        player.transform.parent = currentDollyCart.transform; //Moves playerPos to the dollyCaet Position
        player.transform.position = player.transform.parent.position - new Vector3(0, 1, 0);
        player.transform.rotation = Quaternion.Euler(player.transform.parent.rotation.x, player.transform.parent.rotation.y, player.transform.parent.rotation.z);

        yield return new WaitForSeconds(1f);
        cinematicMovementStopped = false;

        currentCinematicDirector.Play();

        StopCoroutine(setCinematicPosition());
    }

    private IEnumerator playFreeTheLightEffect() //Gives times to play effect / waitForSeconds should be changed depending on lenght of effect
    {
        animHook.setSpeed(0F); //Lerp?
        cinematicMovementStopped = true;
        if(interactKeyPressSet == false)
        {
            setInteractState();
        }

        while (interactKeyPressRequired) //Waiting for player input to activate effects
        {
            Debug.Log("Requires key press");
            yield return null;
        }

        //PLAY EFFECTS HERE
        currentLightPillar.activateLightPillar();
        if(musicChangeCalled == false) //Makes sure to only call changeSong once
        {
            BGMusicSelector.instance.changeSong(2);
            musicChangeCalled = true;
        }
        interactKeyPressRequired = false;

        yield return new WaitForSeconds(3f); //Whilst activate animation is playing
        UIManager.instance.startFade(5f, true);

        yield return new WaitForSeconds(1f); //Whilst fading move pos
        player.transform.position = currentPlayerResetPos.position;
        player.transform.rotation = Quaternion.LookRotation(currentPlayerResetPos.forward, Vector3.zero);
        animHook.setInteractBool(false);

        yield return new WaitForSeconds(.5f); //delays the unpausing code to give time for player Pos to be moved

        GameEvents.instance.CinematicTriggerExit(); //Calls the cinematic end 

        yield return new WaitForSeconds(3.1f);

        animHook.setBool("StandingUp_fromCrouch", true);

        yield return new WaitForSeconds(3.5f);

        animHook.setBool("StandingUp_fromCrouch", false);

        StopCoroutine(playFreeTheLightEffect());
    }

    private IEnumerator resetPlayerDrivenCinematicComponenets() //Resetting values of cinematicController
    {
        PlayerManager.instance.setCameraLookAt(lookAtTarget, false);
        yield return new WaitForSeconds(2.5f);

        if (currentCinematicCam != null)
        {
            currentCinematicCam.Priority = 6; //Switches to mainCam
        }

        playerMainCam.Priority = 15;

        yield return new WaitForSeconds(5f); //Delays player movement after whiteOut          

        cinematicMovementStopped = false;

        currentCinematicType = CinematicType.PLAYERDRIVEN;
        currentDollyCart = null;
        currentCinematicCam = null;
        currentCinematicDirector = null;
        currentLightPillar = null;
        currentPlayerResetPos = null;
        lookAtTarget = null;        

        timeIntoAnimation = 0;
        musicChangeCalled = false;
        //CHANGE CINEMACHINE CAMERA OFFSET
                
        yield return new WaitForSeconds(2f);
        PlayerManager.instance.setCameraLookAt(PlayerManager.instance.playerLookatTarget, true);
        yield return new WaitForSeconds(.5f);
        PlayerManager.instance.pausePlayer(false, false);


        StopCoroutine(resetPlayerDrivenCinematicComponenets());
    }

    private void setInteractState()
    {
        interactKeyPressRequired = true;
        interactKeyPressSet = true;
    }
    #endregion

    #region STANDARD CINEMATICS

    private IEnumerator toggleCinematicModels(bool _toggleOn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        currentCinematicPlayerModel.SetActive(_toggleOn);
        currentCinematicCompanionModel.SetActive(_toggleOn);
    }

    private IEnumerator playStandardCinematic()
    {
        yield return new WaitForSeconds(.5f);
        currentCinematicDirector.Play();

        StopCoroutine(playStandardCinematic());
    }

    private IEnumerator resetStandardCinematicComponents()
    {
        yield return new WaitForSeconds(1f);
        UIManager.instance.startFade(4f, true);
        playerMainCam.Priority = 15;

        StartCoroutine(toggleCinematicModels(false, 1f));

        yield return new WaitForSeconds(2f);

        currentCinematicPlayerModel = null;
        currentCinematicCompanionModel = null;
        PlayerManager.instance.pausePlayer(false, false);

    }

    #endregion
}

public enum CinematicType
{
    PLAYERDRIVEN,
    STANDARD   
}
