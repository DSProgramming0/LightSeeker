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

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private bool pauseMenuActive = false;


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
            if (blackoutScreen.alpha != 0)
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

        togglePauseMenu();
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

    public void ToggleMouse(bool _turnOn)
    {
        if (_turnOn)
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

    public void togglePauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuActive == false)
            {
                togglePlayerState(true);
                pauseMenu.SetActive(true);
                pauseMenuActive = true;
            }
            else if (pauseMenuActive == true)
            {
                togglePlayerState(false);
                pauseMenu.SetActive(false);
                pauseMenuActive = false;

            }
        }
    }

    private void togglePlayerState(bool _toggleOn) //Controls player state and UI whilst in a dialog scene
    {
        if (_toggleOn)
        {
            ToggleMouse(true);
            hideCrosshair(true);
            PlayerManager.instance.pausePlayer(true, false);
        }
        else
        {
            hideCrosshair(false);
            ToggleMouse(false);
            PlayerManager.instance.pausePlayer(false, false);
        }
    }
}

