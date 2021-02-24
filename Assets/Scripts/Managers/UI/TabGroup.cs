using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;

    public TabButton selectedTab;
    public List<GameObject> objectsToSwap;
    [SerializeField] private int index;

    [Header("Audio")]
    [SerializeField] private AudioSource clickAudioSouce;
    [SerializeField] private AudioClip clickSoundEffect;

    public void Subscribe(TabButton button) //Called by each tabButton on start to add them to a new list of TabButtons
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabButton>(); //Create new List of TabButtons
        }

        tabButtons.Add(button); //Add tabButtons that call this method and are passed as an argument
    }

    public void OnTabEnter(TabButton button) //When the mouse hovers over a tab button
    {
        ResetTabs();
        if(selectedTab == null || button != selectedTab)
        {
            button.backgroundImage.color = button.hoverColor; //swap color to hover color
        }
    }

    public void OnTabExit(TabButton button) //reset color when mouse has left tab button area
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button) //When a tab button is pressed down
    {
        if(selectedTab != button) //Only if the selected tab is not equal to the tab being selected
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

            index = button.transform.GetSiblingIndex();
            for (int i = 0; i < objectsToSwap.Count; i++) //cycles through list of pages and sets the corresponding page
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

    public void cyclePage(bool _shouldIncrement) //Need to finish, button cycling
    {
        if (_shouldIncrement)
        {
            index++;
        }
        else
        {
            index--;
        }

        OnTabSelected(selectedTab);
    }

    public void ResetTabs() //resets buttons that are not selected or being hovered
    {
        foreach(TabButton button in tabButtons)
        {
            if(selectedTab != null && button == selectedTab) { continue; }
            button.backgroundImage.color = button.idleColor;
        }
    }
}
