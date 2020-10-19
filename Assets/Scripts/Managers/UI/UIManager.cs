using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private Image crosshair;
    [SerializeField] private CanvasGroup blackoutScreen;
    [SerializeField] private CanvasGroup whiteOutScreen;

    [SerializeField] private float fadeTime;
    [SerializeField] private bool fadeInBlack;
    [SerializeField] private bool fadeInWhite;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (fadeInBlack)
        {
           DoFadeBlack();
        }
        else
        {
            if(blackoutScreen.alpha != 0)
            {
                UnFadeBlack();
            }
        }

        if (fadeInWhite)
        {
            DoFadeWhite();
        }
        else
        {
            if (whiteOutScreen.alpha != 0)
            {
                UnFadeWhite();
            }
        }
    }

    #region transitions

    public void startFade(float _time, bool _whiteOut)
    {
        if (!_whiteOut)
        {
            StartCoroutine(toggleBlackout(_time));
        }
        else
        {
            StartCoroutine(toggleWhiteOut(_time));
        }
    }

    private IEnumerator toggleBlackout(float _blackoutTime)
    {
        fadeInBlack = true;
        yield return new WaitForSeconds(_blackoutTime);
        fadeInBlack = false;

        StopCoroutine(toggleBlackout(0));
    }

    private IEnumerator toggleWhiteOut(float _whiteOutTime)
    {
        fadeInWhite = true;
        yield return new WaitForSeconds(_whiteOutTime);
        fadeInWhite = false;

        StopCoroutine(toggleWhiteOut(0));
    }

    private void DoFadeBlack()
    {
        blackoutScreen.alpha += fadeTime * Time.deltaTime;
    }

    private void UnFadeBlack()
    {
        blackoutScreen.alpha -= fadeTime * Time.deltaTime;
    }

    private void DoFadeWhite()
    {
        whiteOutScreen.alpha += fadeTime * Time.deltaTime;
    }

    private void UnFadeWhite()
    {
        whiteOutScreen.alpha -= fadeTime * Time.deltaTime;
    }

    #endregion

    public void togglePrompt(TextMeshProUGUI _promptToShow, bool _shouldShow)
    {
        if (_shouldShow)
        {
            _promptToShow.alpha = 1;
        }
        else
        {
            _promptToShow.alpha = 0;
        }
    }
    
    public void hideCrosshair(bool _shouldHide)
    {
        if (_shouldHide)
            crosshair.enabled = false;
        else
            crosshair.enabled = true;        
    }

}
