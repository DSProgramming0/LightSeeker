using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private Animator anim;
    public Vector3 lookObj = Vector3.zero;


    [SerializeField] private float DistanceToGround;
    public LayerMask groundMask;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        lookObj = playerInteract.returnMousePos();
    }   

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (anim)
        {
            anim.SetLookAtWeight(1);
            anim.SetLookAtPosition(lookObj);

            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKLeftFootWeight"));
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKLeftFootWeight"));
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKRightFootWeight"));
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKRightFootWeight"));

            RaycastHit hit;
            Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, groundMask))
            {
                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += DistanceToGround;
                    anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
                }
            }

            ray = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, groundMask))
            {
                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += DistanceToGround;
                    anim.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                    anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
                }
            }
        }
    }
}
