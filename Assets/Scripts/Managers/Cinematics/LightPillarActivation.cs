using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPillarActivation : MonoBehaviour
{
    private AnimatorHook animHook;

    private bool hasPlayed;
    private bool playEffect;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Material activatedLightPillarMat;
    [SerializeField] private float alterSpeedValue;



    void Start()
    {
        animHook = GetComponent<AnimatorHook>();
        mesh = GetComponent<MeshRenderer>();
    } 
    
    public void activateLightPillar() //Starts to play all effects related to lightPillar activating
    {
        if (!hasPlayed)
        {
            animHook.setBool("hasActivated", true);
            mesh.material = activatedLightPillarMat;

            hasPlayed = true;
            playEffect = true;

            Target thisIcon = GetComponentInChildren<Target>();
            thisIcon.removeSelf();

            StartCoroutine(stopEffect());
        }
    }

    void Update()
    {
        if(playEffect)
        {
            alterEmission();
        }
    }

    private void alterEmission()
    {
        Debug.Log("Calling");
        float value = Mathf.Lerp(1.1f, 1.9f, Mathf.PingPong(Time.time / alterSpeedValue, 1));
        mesh.material.SetFloat("_EmissionPower", value);
    }

    private IEnumerator stopEffect()
    {
        yield return new WaitForSeconds(5f);
        playEffect = false;
    }

}
