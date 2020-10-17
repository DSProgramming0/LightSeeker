using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public List<string> sentences;
    [SerializeField] private int index;
    public float typeSpeed;

    public GameObject continueButton; 

    public void startDialog()
    {
        if(sentences.Count > 0)
        {
            StartCoroutine(Type());
        }
        else
        {
            Debug.LogError("No sentences to type");
        }
    }

    void Update()
    {
        if(textDisplay.text == sentences[index])
        {
            continueButton.SetActive(true);
        }
    }

    IEnumerator Type()
    {
        ToggleMouse(true);
        PlayerManager.instance.pausePlayer(true, false);

        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    public void NextSentence()
    {
        continueButton.SetActive(false);

        if(index < sentences.Count - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
        {
            textDisplay.text = "";
            StopCoroutine(Type());
            ToggleMouse(false);
            PlayerManager.instance.pausePlayer(false, false);

        }
    }

    public void clearSentences()
    {
        index = 0;
    }

    public void setSentences(List<string> _sentences)
    {
        sentences = _sentences;
    }

    private void ToggleMouse(bool _turnOn)
    {
        if(_turnOn)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }     
}
