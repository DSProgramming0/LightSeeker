using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenMessage : MonoBehaviour
{
    public string _name;
    public bool messageRevealed = false;
   
    public void revealMessage()
    {
        Debug.Log("Message reavealed on " + _name);
        messageRevealed = true;
    }
}
