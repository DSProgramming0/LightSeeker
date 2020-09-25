using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPillarActivation : MonoBehaviour
{
    private bool hasPlayed;
    private bool playEffect;
    [SerializeField]  private MeshRenderer mesh;
    [SerializeField] private Material thisMat;
    [SerializeField] private float value1;
    [SerializeField] private float value2;
    [SerializeField] private float value3;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        thisMat = mesh.materials[0];
    }

    void OnTriggerEnter(Collider other) //Calling the cinematic events
    {
        if (other.gameObject.tag == "Player") 
        {
            if (!hasPlayed)
            {
                Debug.Log("Play massive pillar effect, adjust exposure of skybox, make it look fancy");
                hasPlayed = true;
                playEffect = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") ;
        {
            playEffect = false;
        }
    }

    void Update()
    {
        if (playEffect) //Changes intensity of HDR emission to be brighter and blinding
        {
            float intensityMultiplier = Mathf.Pow(value1, value2);
            intensityMultiplier = Mathf.Pow(intensityMultiplier, value3);
            Color color = thisMat.color;

            thisMat.SetColor("_EmissionColor", color *= intensityMultiplier );
        }       
    }

}
