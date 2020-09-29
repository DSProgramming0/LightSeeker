using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HiddenMessage : MonoBehaviour
{
    public string _name;
    public bool messageRevealed;
    [SerializeField] private Image messageGO;
    [SerializeField] private Texture message;
    [SerializeField] private Material mat_ToSet;
    [SerializeField] private Material setMaterial;
    [SerializeField] private float revealTime;
    private float value;
    private bool startReveal;

    void Start()
    {
        Material m = new Material(mat_ToSet);
        messageGO.material = Instantiate(m); //Creates an instance of the material which stops others being effected
        setMaterial = messageGO.material;

        value = setMaterial.GetFloat("_DissolveControl");
        setMaterial.SetTexture("_MainTexture", message); //Sets texture of material
    }

    public void revealMessage() //When player has looked at this obj long enough in focused mode, play dissolve effect and reveal message
    {
        Debug.Log("Message reavealed on " + _name);
        messageRevealed = true;
        startReveal = true;
    }

    private void Update()
    {
        if (startReveal && setMaterial.GetFloat("_DissolveControl") > 0)
        {
            value -= revealTime * Time.deltaTime; 
            setMaterial.SetFloat("_DissolveControl", value); //Decreased dissolve control overtime which plays effect
        }        
    }
}
