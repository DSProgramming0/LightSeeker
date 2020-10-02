using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerInteract : MonoBehaviour
{
    private PlayerManager _playerManager;
    private CompanionData companionData;
    [SerializeField] private Camera cam;
    [SerializeField] private CinemachineFreeLook vCam;
    [SerializeField] private GameObject objectInSight;
    [SerializeField] private GameObject previousObj;
    [SerializeField] private LayerMask playerLayer;

    [Header("Overlay")]
    //this is your object that you want to have the UI element hovering over
    [SerializeField] private OffScreenIndicator offScreenIndicator;
    [SerializeField] private List<GameObject> allActiveLightPillars;
    [SerializeField] private GameObject closestLightPillar;

    [Header("Focusing")]
    private float originalFOV;
    [SerializeField] private float focusedFOV;
    [SerializeField] private float focusSmoothing;

    [Header("Object interaction")]
    [SerializeField] private float InteractDistance;
    [SerializeField] private bool canInteract = false;
    [SerializeField] private LayerMask lookAtLayer;
    [SerializeField] private float interactDistance;
    [SerializeField] private float wallRevealDistance;
    [SerializeField] private float revealingTimer;

    [Header("Companion")]
    private bool targetSwitched = false;


    private void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        companionData = FindObjectOfType<CompanionData>();
        offScreenIndicator = FindObjectOfType<OffScreenIndicator>();
        originalFOV = vCam.m_Lens.FieldOfView;
    }

    private void InteractWithObj()
    {
        if (objectInSight != null)
        {
            objectInSight.GetComponent<InteractableObj>().Interact();
        }
    }   

    public Vector3 returnMousePos()
    {
        RaycastHit cameraHit;
        Ray cameraAim = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Physics.Raycast(cameraAim, out cameraHit, 1000f, lookAtLayer);
        Debug.DrawLine(cameraAim.origin, cameraAim.GetPoint(20f), Color.green);
        float cameraDistance = cameraHit.distance;

        if (_playerManager.getWorldState() == PlayerWorldState.FREECONTROL)
        {
            if (Physics.Raycast(cameraAim, out cameraHit , lookAtLayer))
            {
                Vector3 cameraHitPoint = cameraHit.point;

                if (cameraHit.transform.GetComponent<InteractableObj>()) //An interactable objects has been targets and can now be interacted with
                {
                    float dist = Vector3.Distance(transform.position, cameraHit.transform.position);

                    if (dist <= InteractDistance)
                    {
                        canInteract = true;
                    }
                    else
                    {
                        canInteract = false;
                    }

                    if (canInteract)
                    {
                        objectInSight = cameraHit.transform.gameObject; //Setting object as currentObject in sight
                        previousObj = objectInSight; //Keeping track of object so that the material can be swapped after currentObj becomes null

                        objectInSight.GetComponent<InteractableObj>().HighlightObject(true);
                        if (Input.GetKeyDown(KeyCode.E) && objectInSight != null)
                        {
                            InteractWithObj();
                        }

                    }
                    else
                    {
                        return cameraHitPoint;
                    }

                    return cameraHitPoint;
                }
                else if (cameraHit.transform.GetComponent<HiddenMessage>() && _playerManager.getInteractState() == PlayerInteractState.FOCUSING) //If cameraHit a wall with a secret message and is focusing
                {
                    float dist = Vector3.Distance(transform.position, cameraHit.transform.position);
                    HiddenMessage secretMessageWall = cameraHit.transform.GetComponent<HiddenMessage>();

                    if (dist <= wallRevealDistance)
                    {
                        canInteract = true;
                    }
                    else
                    {
                        canInteract = false;
                    }

                    if (canInteract)
                    {
                        if(secretMessageWall.messageRevealed == false) //If the message has not already been revealed
                        {
                            revealingTimer += Time.deltaTime;

                            if (revealingTimer > 3f) //If player has looked at the message long enough, it will reveal
                            {
                                secretMessageWall.revealMessage();
                            }
                        }
                        else
                        {
                            Debug.Log("You have already discovered the message on " + secretMessageWall._name);
                            revealingTimer = 0; //resets if the player looks away
                        }                        
                    }
                }
                else if(cameraHit.transform.tag == "Companion")
                {
                    Vector3 lookAtPoint = cameraAim.GetPoint(20f);

                    Debug.Log("Intersecting with companion");

                    if (targetSwitched == false)
                    {
                        companionData.switchTarget();
                        targetSwitched = true;
                        StartCoroutine(resetTargetSwitched());
                    }

                    return lookAtPoint;
                }
                else if (cameraHit.transform.tag == "Player" || cameraHit.transform.tag == "Cinematic")
                {
                    Debug.Log("Intersecting with player");

                    Vector3 lookAtPoint = cameraAim.GetPoint(20f);
                    return lookAtPoint;
                }
                else //If we are still looking at something but it is not an interactable Object
                {
                    //Debug.Log("Hitting " + cameraHit.transform.name);
                    revealingTimer = 0;
                    canInteract = false;
                    if (previousObj != null)
                    {
                        previousObj.GetComponent<InteractableObj>().HighlightObject(false); // switch to standard material
                        objectInSight = null;
                    }

                    if (Input.GetKeyDown(KeyCode.E) && objectInSight == null)
                    {
                        Debug.Log("Look at an object to select it!");
                    }
                }
                return cameraHitPoint;
            }
            else //If raycast is going into nothingness/Skybox
            {
                Vector3 cameraHitPoint = cameraAim.GetPoint(20f);

                Physics.Raycast(transform.position + new Vector3(0f, 1.8f, 0f), cameraHitPoint - transform.position - new Vector3(0f, 1.8f, 0f), 100f);
                objectInSight = null;
                canInteract = false;
                revealingTimer = 0;

                if (previousObj != null)
                {
                    previousObj.GetComponent<InteractableObj>().HighlightObject(false);
                }

                if (Input.GetKeyDown(KeyCode.E) && objectInSight == null)
                {
                    Debug.Log("Look at an object to select it!");
                }

                return cameraHitPoint;
            }
        }
        else
        {
            return Vector3.zero;
        }        
    }

    void Update()
    {
        if (Input.GetMouseButton(1) && _playerManager.getCurrentMovmementState() != PlayerMovementStates.RUNNING && _playerManager.getWorldState() == PlayerWorldState.FREECONTROL)
        {
            _playerManager.setInteractState(PlayerInteractState.FOCUSING);
            float fovTargetValue = Mathf.Lerp(vCam.m_Lens.FieldOfView, focusedFOV, focusSmoothing * Time.deltaTime);
            vCam.m_Lens.FieldOfView = fovTargetValue;
        }
        else
        {
            _playerManager.setInteractState(PlayerInteractState.NOTFOCUSING);
            float fovTargetValue = Mathf.Lerp(vCam.m_Lens.FieldOfView, originalFOV, focusSmoothing * Time.deltaTime);
            vCam.m_Lens.FieldOfView = fovTargetValue;
        }

        closestLightPillar = GetClosestLightPillar();
        if(closestLightPillar != null)
        {
            offScreenIndicator.getCurrentTarget(closestLightPillar);
        }
    }

    public void removeFromList(Target _target)
    {
        allActiveLightPillars.Remove(_target.gameObject);
    }

    private IEnumerator resetTargetSwitched()
    {
        yield return new WaitForSeconds(5f);
        targetSwitched = false;
        StopCoroutine(resetTargetSwitched());
    }

    GameObject GetClosestLightPillar()
    {
        GameObject tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in allActiveLightPillars)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }    
}
