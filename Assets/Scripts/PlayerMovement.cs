using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;
    [SerializeField] private AnimatorHook animHook;

    [SerializeField] private float speed;
    private float walkSpeed = 4f;
    private float runSpeed = 8f;
    private float idleSpeed = 0.01f;
    [SerializeField] private float speedGain = 0.01f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;

        if (Input.GetButton("Sprint"))
        {
            float newSpeed = Mathf.Lerp(speed, runSpeed, speedGain * Time.deltaTime);
            speed = newSpeed;
        }
        else if(!Input.GetButton("Sprint") && dir.magnitude <= 0.09)
        {
            float newSpeed = Mathf.Lerp(speed, idleSpeed, speedGain * Time.deltaTime);
            speed = newSpeed;
        }
        else
        {
            float newSpeed = Mathf.Lerp(speed, walkSpeed, speedGain * Time.deltaTime);
            speed = newSpeed;
        }     

        animHook.setSpeed(speed);
            
        if(dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }      
    }
}
