using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusicSelector : MonoBehaviour
{
    public static BGMusicSelector instance;
    private SoundManager soundManager;
    [SerializeField] private AudioClip[] backgroundMusicArray;

    [SerializeField] private int clipIndex;
    [SerializeField] private bool overrridingSong;

    // Start is called before the first frame update
    void Awake()
    {
        soundManager = GetComponent<SoundManager>();

        instance = this;
    }

    void Start()
    {
        //sets random clipIndex and starts playing
        clipIndex = Random.Range(0, backgroundMusicArray.Length -1);
        soundManager.PlayMusic(backgroundMusicArray[clipIndex]);
    }
    // Update is called once per frame
    void Update()
    {
        if(overrridingSong == false) //if we are not trying to override the musicSource
        {
            if (soundManager.isPlaying() == false) //if nothing is playing
            {
                //checking clipIndex
                if (clipIndex >= backgroundMusicArray.Length - 1)
                {
                    clipIndex = 0;
                }
                else
                {
                    clipIndex++;
                }

                soundManager.PlayMusic(backgroundMusicArray[clipIndex]); //Play current index
            }
        }
        else
        {
            soundManager.PlayMusic(backgroundMusicArray[clipIndex]); //if we have overriden, play new song
        }       
    }

    public void changeSong(int _newClipIndex) //Called by other scripts (cinematic controller) to override current song and play new selected one.
    {
        overrridingSong = true;
        clipIndex = _newClipIndex;

        StartCoroutine(resetOverride());
    }

    private IEnumerator resetOverride()
    {
        yield return new WaitForSeconds(.2f);
        overrridingSong = false;
    }
}
