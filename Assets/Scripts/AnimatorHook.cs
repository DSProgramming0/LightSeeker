using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHook : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    
    public void setSpeed(float _speed)
    {
        anim.SetFloat("Speed", _speed);
    }

    public void toggleIK(float _IKValue)
    {
        anim.SetFloat("IKLeftFootWeight", _IKValue);
        anim.SetFloat("IKRightFootWeight", _IKValue);
    }

    public void setJump(float _VelocityY, float _VelocityX)
    {
        anim.SetFloat("VelocityY", _VelocityY);
        anim.SetFloat("VelocityX", _VelocityY);
    }   

    public void setGround(bool _isGrounded)
    {
        anim.SetBool("isGrounded", _isGrounded);
    }

}
