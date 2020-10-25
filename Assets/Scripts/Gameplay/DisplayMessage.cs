using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMessage : MonoBehaviour
{
    [SerializeField] private Image messageDisplay;
    [SerializeField] private InteractableObj thisInteractableObj;
    [SerializeField] private UITweener BackGroundTweener;
    [SerializeField] private UITweener MessageTweener;
    [SerializeField] private GameObject continueButton;

    private void showMessage()
    {
        if(messageDisplay.sprite != null)
        {
            togglePlayerState(true);
            continueButton.SetActive(true);
        }
    }

    public void hideMessage()
    {
        togglePlayerState(false);
        continueButton.SetActive(false);
        thisInteractableObj.callResetInteract();
    }
    
    public void setImage(Sprite _imageToDisplay, InteractableObj _objInteractedWith)
    {
        messageDisplay.sprite = _imageToDisplay;
        thisInteractableObj = _objInteractedWith;

        showMessage();
    }   

    private void togglePlayerState(bool _toggleOn) //Controls player state and UI whilst in a dialog scene
    {
        if (_toggleOn)
        {
            MessageTweener.showUI();
            BackGroundTweener.fadeInBackground();
            UIManager.instance.ToggleMouse(true);
            UIManager.instance.hideCrosshair(true);
            PlayerManager.instance.pausePlayer(true, false);
        }
        else
        {
            MessageTweener.hideUI();
            BackGroundTweener.fadeOutBackground();
            UIManager.instance.hideCrosshair(false);
            UIManager.instance.ToggleMouse(false);
            PlayerManager.instance.pausePlayer(false, false);
        }

    }
}
