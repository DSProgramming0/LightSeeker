using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;

    public TabButton selectedTab;
    public List<GameObject> objectsToSwap;

    [Header("Audio")]
    [SerializeField] private AudioSource clickAudioSouce;
    [SerializeField] private AudioClip clickSoundEffect;

    public void Subscribe(TabButton button)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if(selectedTab == null || button != selectedTab)
        {
            button.backgroundImage.color = button.hoverColor;
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        if(selectedTab != button)
        {
            if (selectedTab != null)
            {
                selectedTab.DeSelect();
            }

            selectedTab = button;

            selectedTab.Select();

            ResetTabs();

            button.backgroundImage.color = button.pressedColor;
            clickAudioSouce.PlayOneShot(clickSoundEffect);

            int index = button.transform.GetSiblingIndex();
            for (int i = 0; i < objectsToSwap.Count; i++)
            {
                if (i == index)
                {
                    objectsToSwap[i].SetActive(true);
                }
                else
                {
                    objectsToSwap[i].SetActive(false);
                }
            }
        }       
    }

    public void ResetTabs()
    {
        foreach(TabButton button in tabButtons)
        {
            if(selectedTab != null && button == selectedTab) { continue; }
            button.backgroundImage.color = button.idleColor;
        }
    }
}
