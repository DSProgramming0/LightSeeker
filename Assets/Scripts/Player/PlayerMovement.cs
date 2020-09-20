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
    public float groundDistance = 0.8f;


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
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
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
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }
}
