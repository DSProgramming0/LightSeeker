using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;
    [SerializeField] private AnimatorHook animHook;

    [SerializeField] private Transform groundCheck;
    Vector3 velocity;
    public bool isGrounded;
    public LayerMask groundMask;
    public float gravity = -9.81f;
    public float groundDistance = 0.45f;
    [SerializeField] private float slopeForceRayLength;
    [SerializeField] private float adjustmentSmoothTime;
    [SerializeField] private float moveGroundCheckDist;

    [SerializeField] private float distToGroundCheck;
    [SerializeField] private float distMax;
    [SerializeField] private float distMin;

    [SerializeField] private float speed;
    private float walkSpeed = 2f;
    private float runSpeed = 8f;
    private float idleSpeed = 0.01f;
    [SerializeField] private float speedGain;
    [SerializeField] private float walkToIdleGain = 8f;
    [SerializeField] private float idleToWalkGain = 8f;
    [SerializeField] private float walkToRunGain = 8f;
    [SerializeField] private float turnSmoothTime;
    private float standardTurnSmooth = 0.4f;
    private float sprintTurnSmooth = 0.05f;
    float turnSmoothVelocity;

    // Update is called once per frame
    void FixedUpdate()
    {
       
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded)
        {
            MoveGroundCheck();
        }

        Debug.DrawLine(new Vector3(transform.position.x,transform.position.y +.93f, transform.position.z), groundCheck.position, Color.red);
        distToGroundCheck = transform.position.y + .93f - groundCheck.position.y;
        Debug.Log("distToGroundCheck "+ distToGroundCheck);
        if(distToGroundCheck < 0.79f)
        {
            Debug.Log("adjusting...");

            transform.position = new Vector3(transform.position.x, transform.position.y + distToGroundCheck * Time.fixedDeltaTime, transform.position.z);
        }
        else if(distToGroundCheck > 0.85f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - distToGroundCheck * Time.fixedDeltaTime, transform.position.z);
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;

        if (Input.GetButton("Sprint") && dir.magnitude >= 1)
        {
            //Running
            speedGain = walkToRunGain;
            float newSpeed = Mathf.Lerp(speed, runSpeed, speedGain * Time.deltaTime);
            speed = newSpeed;
            turnSmoothTime = sprintTurnSmooth;
        }
        else if (!Input.GetButton("Sprint") && dir.magnitude <= 0.09)
        {
            //Idle
            speedGain = idleToWalkGain;
            float newSpeed = Mathf.Lerp(speed, idleSpeed, speedGain * Time.deltaTime);
            speed = newSpeed;
            turnSmoothTime = standardTurnSmooth;
        }
        else if(!Input.GetButton("Sprint"))
        {
            //Walking
            speedGain = walkToIdleGain;
            float newSpeed = Mathf.Lerp(speed, walkSpeed, speedGain * Time.deltaTime);
            speed = newSpeed;
            turnSmoothTime = standardTurnSmooth;
        }

        animHook.setSpeed(speed);

        Debug.Log(dir.magnitude);

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (!isGrounded)
        {
            animHook.toggleIK(0f);
            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }            
    }

    private void MoveGroundCheck()
    {
        RaycastHit hit;
        Debug.DrawLine(groundCheck.position, groundCheck.position + (Vector3.down * moveGroundCheckDist), Color.red);

        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, moveGroundCheckDist, groundMask))
        {
            Debug.Log("Hitting layer");
            groundCheck.position = new Vector3(groundCheck.position.x, hit.point.y + 0.1f, groundCheck.position.z);
        }

        if(Physics.Raycast(groundCheck.position, Vector3.up, out hit, 6f, groundMask))
        {
            Debug.Log("BelowGround");
        }
    }

    private bool OnSlope()
    {
        RaycastHit hit;
        Debug.DrawLine(groundCheck.position, groundCheck.position + (Vector3.down * slopeForceRayLength), Color.red);

        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, slopeForceRayLength))
        {
            if (hit.transform.rotation.z != 0)
            {
                return true;
            }
        }

        return false;

    }
}
