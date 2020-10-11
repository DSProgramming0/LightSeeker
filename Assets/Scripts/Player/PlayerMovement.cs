using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerManager _playerManager;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;
    [SerializeField] private AnimatorHook animHook;

    Vector3 velocity;     

    private float horizontal;
    private float vertical;
    private Vector3 dir;

    [Header("Standard Movement")]
    [SerializeField] private float speed;
    private float walkSpeed = 2f;
    private float runSpeed = 7f;
    private float idleSpeed = 0.01f;
    [SerializeField] private float speedGain;
    [SerializeField] private float walkToIdleGain;
    [SerializeField] private float idleToWalkGain;
    [SerializeField] private float walkToRunGain;
    [SerializeField] private float turnSmoothTime;
    private float standardTurnSmooth = 0.4f;
    private float sprintTurnSmooth = 0.05f;
    float turnSmoothVelocity;

    [Header("Jumping")]
    [SerializeField] private bool isJumping;
    [SerializeField] private float jumpHeight;
    public bool isGrounded;
    [SerializeField] private Transform groundCheck;
    public LayerMask groundMask;
    public float gravity = -9.81f;
    public float groundDistance = 0.45f;

    [Header("Checks")]
    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;
    [SerializeField] private float adjustmentSmoothTime;
    [SerializeField] private float moveGroundCheckDist;
    [SerializeField] private float distToGroundCheck;
    [SerializeField] private float distMax;
    [SerializeField] private float distMin;

    [Header("Controls")]
    [SerializeField] private bool stopPlayerMovement = false;
    public bool isRunning = false;

    void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
    }

    void FixedUpdate()
    {    
        if(_playerManager.getWorldState() != PlayerWorldState.INCINEMATIC)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //Checking GroundCheck by casting a sphere, if it hits the correct layer, player is grounded

            //if (isGrounded)
            //{
            //    MoveGroundCheck(); //Constantly moves the groundCheck along the surface below the player, if they're grounded
            //}

            Debug.DrawLine(new Vector3(transform.position.x, transform.position.y + .93f, transform.position.z), groundCheck.position, Color.red);
            distToGroundCheck = transform.position.y + .93f - groundCheck.position.y; //Checking the distance between the midway of the controller to the groundcheck

            if (!stopPlayerMovement) //Control for if the palyer can move
            {
                dir = new Vector3(horizontal, 0f, vertical).normalized; //Uses the horizontal and vertical inputs to create a vector3 value, which is used in the controllers move function
            }
            else
            {
                dir = Vector3.zero;
            }

            if (dir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f); //Uses the angle of the player in relation to the camera and applies its to the rotation of the player

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime); //Moves the palyer in the direction specified by its rotation.

                //if (!isJumping)
                //{
                //    if (distToGroundCheck < 0.79f) //Adjusts players leg IK if the distance to groundCheck gets below a certain value.
                //    {
                //        Debug.Log("adjusting...");

                //        transform.position = new Vector3(transform.position.x, transform.position.y + distToGroundCheck * Time.fixedDeltaTime, transform.position.z);
                //    }
                //    else if (distToGroundCheck > 0.85f)
                //    {
                //        Debug.Log("lowering");

                //        transform.position = new Vector3(transform.position.x, transform.position.y - distToGroundCheck * Time.fixedDeltaTime, transform.position.z);
                //    }
                //}
            }

            if (isJumping) //Applies force on the yAxis to simulate a jump
            {
                velocity.y = Mathf.Sqrt(jumpHeight * 5);

                controller.Move(velocity * Time.fixedDeltaTime);

                if (isGrounded) //reset values if grounded
                {
                    if (stopPlayerMovement)
                    {
                        StartCoroutine(delayMovementAfterJump()); //Delays movement after a idle Jump
                    }

                    animHook.setGround(true);
                    isJumping = false;
                }               
            }
            
            velocity.y += gravity * Time.fixedDeltaTime;

            controller.Move(velocity * Time.fixedDeltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerManager.getWorldState() != PlayerWorldState.INCINEMATIC)
        {
            //Storing player input wiht no smoothing applied
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if (Input.GetButton("Sprint") && dir.magnitude >= 1) //Lerps speed depending on player Input, works with blend tree to blend animations and smooth rotations
            {
                //Running      
                _playerManager.setMovementState(PlayerMovementStates.RUNNING);
                speedGain = walkToRunGain;
                float newSpeed = Mathf.Lerp(speed, runSpeed, speedGain * Time.deltaTime);
                speed = newSpeed;
                turnSmoothTime = sprintTurnSmooth;
            }
            else if (!Input.GetButton("Sprint") && dir.magnitude <= 0.09)
            {
                //Idle
                _playerManager.setMovementState(PlayerMovementStates.IDLE);
                speedGain = idleToWalkGain;
                float newSpeed = Mathf.Lerp(speed, idleSpeed, speedGain * Time.deltaTime);
                speed = newSpeed;
                turnSmoothTime = standardTurnSmooth;

            }
            else if (!Input.GetButton("Sprint"))
            {
                //Walking
                _playerManager.setMovementState(PlayerMovementStates.WALKING);
                speedGain = walkToIdleGain;
                float newSpeed = Mathf.Lerp(speed, walkSpeed, speedGain * Time.deltaTime);
                speed = newSpeed;
                turnSmoothTime = standardTurnSmooth;
            }
            else if (_playerManager.getWorldState() == PlayerWorldState.INCINEMATIC)
            {
                Debug.Log("Calling");
                _playerManager.setMovementState(PlayerMovementStates.IDLE);
            }

            if (Input.GetButtonDown("Jump") && isGrounded && !isJumping) //Controls jumping, can only jump if not jumping
            {
                isJumping = true;
                animHook.setGround(false);

                if (dir.magnitude >= 0.1f) //Plays running Jump
                {
                    animHook.setJump(1, 1);
                }
                else if (dir.magnitude == 0) //Plays idle JUmp
                {
                    stopMovement(true);

                    animHook.setJump(0, 1);
                }
            }

            animHook.setSpeed(speed);
        }
    }

    //private void MoveGroundCheck() //Moves groundCheck Obj to skim across the surface below the player
    //{
    //    RaycastHit hit;
    //    Debug.DrawLine(groundCheck.position, groundCheck.position + (Vector3.down * moveGroundCheckDist), Color.red);

    //    if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, moveGroundCheckDist, groundMask))
    //    {
    //        groundCheck.position = new Vector3(groundCheck.position.x, hit.point.y + 0.1f, groundCheck.position.z);
    //    }

    //    if(Physics.Raycast(groundCheck.position, Vector3.up, out hit, 6f, groundMask))
    //    {
    //        Debug.Log("BelowGround");
    //    }
    //}

    //private bool OnSlope() //Checks if the player is on a slope
    //{

    //    if (isJumping)
    //    {
    //        return false;
    //    }

    //    RaycastHit hit;
    //    Debug.DrawLine(transform.position, transform.position + (Vector3.down * slopeForceRayLength), Color.red);

    //    if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength))
    //    {
    //        if(hit.normal != Vector3.up)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    public void stopMovement(bool _stopPlayer) //Controls player movement
    {
        stopPlayerMovement = _stopPlayer;
    }

    private IEnumerator delayMovementAfterJump() //delays movement after an idle jump
    {
        yield return new WaitForSeconds(1f);
        stopMovement(false);
        StopCoroutine(delayMovementAfterJump());
    }
}
