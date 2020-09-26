using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime;
    [SerializeField] private float movementModifier;
    [SerializeField] private float offset;

    void Start()
    {
        if (player)
        {
            player = FindObjectOfType<PlayerManager>().transform;
        }
    }

    void FixedUpdate()
    {
        Vector3 offsetFollowPos = new Vector3(player.position.x, player.position.y + offset, player.position.z);
        transform.position = Vector3.Lerp(transform.position, offsetFollowPos, (movementModifier * Time.deltaTime) / smoothTime ); //Lerps the position of this gameObject to the player position over time
    }
}
