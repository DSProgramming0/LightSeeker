using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionBehaviour : MonoBehaviour
{
    private CompanionData data;

    private Transform target;
    [SerializeField] private Light flashLight;

    [SerializeField] private bool isInspecting;
    [SerializeField] private bool isAnalyzing;
    [SerializeField] private bool isSpeaking;

    [SerializeField] private bool lightIsOn;
    [SerializeField] private bool flashlightWasOn = false;

    [SerializeField] private float inspectingTime;
    [SerializeField] private float atInspectionTime;


    private CompanionStates state;
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.instance.onCompanionChanged += targetChanged;

        data = GetComponent<CompanionData>();
        flashLight = GetComponentInChildren<Light>();

        target = data.Targets;

        state = CompanionStates.IDLE;
    }
    
    private void targetChanged()
    {
        target = data.Targets;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case CompanionStates.IDLE:
                
                break;
            case CompanionStates.INSPECTING:
                break;
            case CompanionStates.ANALYSING:
                break;
            case CompanionStates.SPEAKING:
                break;
            default:
                break;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            state = CompanionStates.INSPECTING;
            atInspectionTime = Time.time + inspectingTime;
            isInspecting = true;
        }

        if (isInspecting && Time.time > atInspectionTime)
        {
            isInspecting = false;
            state = CompanionStates.IDLE;
            atInspectionTime = 0;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Calling");
            toggleFlashLight();
        }       
    }

    private void toggleFlashLight()
    {
        if (lightIsOn)
        {
            lightIsOn = false;
            flashLight.enabled = false;
        }
        else if(lightIsOn == false)
        {
            lightIsOn = true;
            flashLight.enabled = true;
        }
    }

}
