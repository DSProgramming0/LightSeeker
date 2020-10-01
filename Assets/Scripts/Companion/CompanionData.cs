using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionData : MonoBehaviour
{
    [SerializeField] private Transform[] targets;
    [SerializeField] private Animator anim;

    [SerializeField] private Vector3 companionOffset;
    [SerializeField] private float delaySeconds = 0.5f;

    private int targetIndex = 0;

    [SerializeField] private float alternatePosTimer = 10f;
    private Vector3 focusedOffset_1 = new Vector3(.1f, 0.1f, -.3f);
    private Vector3 focusedOffset_2 = new Vector3(0f, 0.1f, .3f);
    private Vector3 standardOffset = new Vector3(0, 0, 0);

    [SerializeField] private Material mat_ToSet;
    [SerializeField] private Material setMaterial;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private float value = 0f;
    [SerializeField] private float revealTime;
    [SerializeField] private bool phaseAway;
    [SerializeField] private bool phaseIn;

    void Start()
    {
        anim = GetComponent<Animator>();
        mesh = GetComponentInChildren<MeshRenderer>();

        Material m = new Material(mat_ToSet);
        mesh.material = Instantiate(m); //Creates an instance of the material which stops others being effected
        setMaterial = mesh.material;

        setMaterial.SetFloat("_DissolveControl",0f);
    }

    public Transform Targets
    {
        get { return targets[targetIndex]; }       
    }  

    public Animator Anim
    {
        get { return anim; }
    }

    public Vector3 CompanionOffset
    {
        get { return companionOffset; }
    }
     
    public float DelaySeconds
    {
        get { return delaySeconds; }
    }

    void Update()
    {
        if(PlayerManager.instance.getInteractState() == PlayerInteractState.FOCUSING)
        {
            delaySeconds = 1f;

            if(targetIndex == 0)
            {
                companionOffset = focusedOffset_1;
            }
            else
            {
                companionOffset = focusedOffset_2;
            }
        }
        else
        {
            companionOffset = standardOffset;
            delaySeconds = 0.2f;
        }

        if(targetIndex == 1)
        {
            Debug.Log("In alternate pos");
            alternatePosTimer -= Time.deltaTime;

            if(alternatePosTimer <= 0)
            {
                switchTarget();
                alternatePosTimer = 5f;
            }
        }

        if (phaseAway && setMaterial.GetFloat("_DissolveControl") < 0.8f)
        {
            StartCoroutine(startDissolve());           
        }     
           
    }

    public void switchTarget()
    {
        if (targetIndex == 0)
            targetIndex = 1;
        else
            targetIndex = 0;

        GameEvents.instance.CompanionTargetChanged();
        phaseAway = true;
    }  

    private IEnumerator startDissolve()
    {
        value += revealTime * Time.deltaTime;
        setMaterial.SetFloat("_DissolveControl", value);
        yield return new WaitForSeconds(1.5f);
        value -= revealTime * Time.deltaTime;
        setMaterial.SetFloat("_DissolveControl", value);

        phaseAway = false;
    }

}

public enum CompanionStates
{
    IDLE,
    INSPECTING,
    ANALYSING,
    SPEAKING
}
