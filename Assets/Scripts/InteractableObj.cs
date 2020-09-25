using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObj : MonoBehaviour
{
    public string name;
    [SerializeField] private bool shouldHighlight;

    private MeshRenderer mesh;
    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private Material startingMaterial;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();

        startingMaterial = mesh.material;
    }

    void Update()
    {
        if (shouldHighlight)
        {
            mesh.material = highlightedMaterial;
        }
        else
        {
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
