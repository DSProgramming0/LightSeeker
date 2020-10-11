using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableObj : MonoBehaviour
{
    public string name;
    [SerializeField] private TextMeshProUGUI prompt;

    [SerializeField] private bool shouldHighlight;
    private bool startCheck = false;

    private MeshRenderer mesh;
    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private Material startingMaterial;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();

        startingMaterial = mesh.material;

    }

    void check()
    {
        startCheck = true;
    }

    void LateUpdate()
    {

        if (shouldHighlight)
        {
            mesh.material = highlightedMaterial;
            UIManager.instance.togglePrompt(prompt, true);
        }
        else
        {
            UIManager.instance.togglePrompt(prompt, false);
            mesh.material = startingMaterial;
        }

    }

    public void HighlightObject(bool _shouldHighlight)
    {
        shouldHighlight = _shouldHighlight;
    } 

    public void Interact()
    {
        Debug.Log("The player did something to me 0 " + name);
    }

}
