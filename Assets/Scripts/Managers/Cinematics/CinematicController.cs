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

    [Header("Cinemachine")]
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

    void Update()
    {
        if (PlayerManager.instance.getWorldState() == PlayerWorldState.INCINEMATIC)
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

            if(currentCinematicDirector.time >= currentCinematicDirector.duration - 1f) //Starts playing beam of light effect before end of animation
            {
                Debug.Log("Reached light beam");
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
        else
        {
            interactKeyPressSet = false;
            lightBeamActivated = false;
            interactKeyPressRequired = false; //If player is not in a cinematic/ Safety net if statement
            animHook.setInteractBool(false);
        }
    }

    public void setCurrentCinematicComponents(CinemachineDollyCart _dollyCart, CinemachineVirtualCamera _vCam, PlayableDirector _director, LightPillarActivation _lightPillar, Transform _playerResetPos, Transform _lookAtPos)
    { //Sets cinematicControllers components dependent on the current cinematicTrigger entered
        Debug.Log("Called");
        currentDollyCart = _dollyCart;
        currentCinematicCam = _vCam;
        currentCinematicDirector = _director;
        currentLightPillar = _lightPillar;
        currentPlayerResetPos = _playerResetPos;
        lookAtTarget = _lookAtPos;
    }

    public void pausePlayerCinematic() //Pauses player controls, sets them as a child of the card and switched camera
    {
        PlayerManager.instance.pausePlayer(true, false);
        PlayerManager.instance.setWorldState(PlayerWorldState.INCINEMATIC);

        UIManager.instance.startFade(.75f, false);

        StartCoroutine(setCinematicPosition());
    }

    private IEnumerator setCinematicPosition() //Delays palyer movement until blackout screen has unfaded and player pos is set in the cinematic dolly cart
    {
        cinematicMovementStopped = true;
        yield return new WaitForSeconds(.2f);
        currentCinematicCam.Priority = 15; //Switches camera to cinematic camera
        currentCinematicDirector.Play();

        Debug.Log("Calling");

        player.transform.parent = currentDollyCart.transform; //Moves playerPos to the dollyCaet Position
        player.transform.position = player.transform.parent.position - new Vector3(0, 1, 0);
        player.transform.rotation = Quaternion.Euler(player.transform.parent.rotation.x, player.transform.parent.rotation.y, player.transform.parent.rotation.z);

        yield return new WaitForSeconds(1f);
        cinematicMovementStopped = false;

        StopCoroutine(setCinematicPosition());
    }

    public void unPausePlayerCinematic() //Resetting values of the cinematic Controller and giving the player control
    {
        PlayerManager.instance.setWorldState(PlayerWorldState.FREECONTROL);

        player.transform.parent = null;

        StartCoroutine(resetCinematicComponenets());
    }

    private void setInteractState()
    {
        interactKeyPressRequired = true;
        interactKeyPressSet = true;
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

    private IEnumerator resetCinematicComponenets() //Resetting values of cinematicController
    {
        PlayerManager.instance.setCameraLookAt(lookAtTarget);
        yield return new WaitForSeconds(2.5f);
        if (currentCinematicCam != null)
        {
            currentCinematicCam.Priority = 8; //Switches to mainCam
        }        

        yield return new WaitForSeconds(5f); //Delays player movement after whiteOut

        cinematicMovementStopped = false;
        currentDollyCart = null;
        currentCinematicCam = null;
        currentCinematicDirector = null;
        timeIntoAnimation = 0;

        yield return new WaitForSeconds(2000f);
        PlayerManager.instance.setCameraLookAt(PlayerManager.instance.playerLookatTarget);
        yield return new WaitForSeconds(.5f);
        PlayerManager.instance.pausePlayer(false, false);


        StopCoroutine(resetCinematicComponenets());

    }
}
