using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class Dialog : MonoBehaviour
{    [Header("Cinemachine")]

    [SerializeField] private CinemachineClearShot dialogCam;

    [Header("Audio")]
    [SerializeField] private AudioSource typingAudioSource;
    public List<AudioClip> typingSounds;
    [SerializeField] private AudioClip selectedTypingSound;
    [SerializeField] private AudioClip continueSound;

    [Header("Dialog UI")]
    [SerializeField] private UITweener tweener;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private List<string> sentences;
    [SerializeField] private int index;    
    [SerializeField] private bool dialogDisplaying;
    private bool DialogSet;

    public float typeSpeed;

    //Checks if there are sentences to type before calling the coroutine
    public void startDialog()
    {
        if(sentences.Count > 0)
        {
            StartCoroutine(Type());
        }
        else //displays an error
        {
            Debug.LogError("No sentences to type");
        }
    }

    void Update()
    {
        if(textDisplay.text == sentences[index]) //Checks when the text in the dialogBox is the same as the current sentence at the current index
        {
            continueButton.SetActive(true); // if true, show the continue button
        }

        if (dialogDisplaying) //Switches cameras
        {
            dialogCam.m_Priority = 20;
        }
        else
        {
            dialogCam.m_Priority = 6;
        }
    }

    IEnumerator Type()
    {
        if (DialogSet == false)
        {
            togglePlayerState(true); //tweens UI, hides crosshair, enables mouse movement and pauses player
            DialogSet = true;
            yield return new WaitForSeconds(.25f);
        }

        foreach (char letter in sentences[index].ToCharArray()) //Types each character in the current sentence
        {
            yield return new WaitForSeconds(typeSpeed); //Types at a delay
            textDisplay.text += letter;
            typingAudioSource.PlayOneShot(selectedTypingSound); //Plays a blip sound with each character typed
        }
    }

    public void NextSentence() //Continues to the next sentence
    {
        continueButton.SetActive(false);
        typingAudioSource.PlayOneShot(continueSound); //Plays sound when pressed

        if (index < sentences.Count - 1) //If the index is less than the value of the list of sentences
        {
            index++; //Increment index
            textDisplay.text = "";
            StartCoroutine(Type()); //Continue typing
        }
        else //stop Typing
        {
            textDisplay.text = "";
            StopCoroutine(Type());

            togglePlayerState(false); //Tween UI, reveals crosshair, disables mouse movement and hides it, unpauses player
        }
    }

    private void togglePlayerState(bool _toggleOn) //Controls player state and UI whilst in a dialog scene
    {
        if (_toggleOn)
        {
            tweener.showUI();
            UIManager.instance.ToggleMouse(true);
            dialogDisplaying = true;
            UIManager.instance.hideCrosshair(true);
            PlayerManager.instance.pausePlayer(true, false);
        }
        else
        {
            tweener.hideUI();
            UIManager.instance.hideCrosshair(false);
            dialogDisplaying = false;
            UIManager.instance.ToggleMouse(false);
            PlayerManager.instance.pausePlayer(false, false);
            DialogSet = false;
        }
    }

    public void resetIndex()
    {
        index = 0;
    }

    public void setSentences(List<string> _sentences) //Sets the sentences which will be typed, called by a dialogTrigger
    {
        sentences = _sentences;
    }

    public void setTypingSound(int soundClipIndex) //Sets the current typing sound to play, called in a dialogTrigger
    {
        selectedTypingSound = typingSounds[soundClipIndex];
    }
    
}
