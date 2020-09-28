using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenMessage : MonoBehaviour
{
    public string _name;
    public bool messageRevealed = false;
   
    public void revealMessage() //When player has looked at this obj long enough in focused mode, play dissolve effect and reveal message
    {
        Debug.Log("Message reavealed on " + _name);
        messageRevealed = true;
    }
}
