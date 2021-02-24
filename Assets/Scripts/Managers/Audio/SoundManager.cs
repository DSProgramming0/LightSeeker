using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Audio players components.
    public AudioSource EffectsSource;
    public AudioSource MusicSource;
    public AudioSource footstepSource;

    // Random pitch adjustment range.
    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    [SerializeField] private float smoothValue;
    [SerializeField] private List<AudioClip> footSteps;

    // Singleton instance.
    public static SoundManager Instance = null;

    // Initialize the singleton instance.
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    // Play a single clip through the sound effects source.
    public void Play(AudioClip clip)
    {
        EffectsSource.clip = clip;
        EffectsSource.Play();
    }

    void Update()
    {
        //if (songNearlyOver())
        //{
        //    blendTransition(true);
        //}
        //else
        //{
        //    blendTransition(false);
        //}
    }

    // Play a single clip through the music source.
    public void PlayMusic(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.Play();
    }   

    // Play a random clip from an array, and randomize the pitch slightly.
    public void RandomSoundEffect(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        EffectsSource.pitch = randomPitch;
        EffectsSource.clip = clips[randomIndex];
        EffectsSource.Play();
    }

    //returns true if music source is playing something.
    public bool isPlaying()
    {
        if (MusicSource.isPlaying)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool songNearlyOver()
    {
        if ((MusicSource.timeSamples / MusicSource.clip.frequency) >= MusicSource.clip.length - 5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Step(bool isLanding)
    {
        AudioClip clip = getRandomFootstep();

        if (!isLanding)
        {
            footstepSource.PlayOneShot(clip);

        }
        else
        {
            footstepSource.PlayOneShot(footSteps[4]);

        }
    }

    public AudioClip getRandomFootstep()
    {
        return footSteps[Random.Range(0, footSteps.Count - 1)];
    }

    //private void blendTransition(bool _blendDown)
    //{
    //    if (_blendDown)
    //    {
    //        if(MusicSource.volume >= 0.1f) //if musicSource is not at 0.1 volume, it will try to lower the volume
    //        {
    //            Debug.Log("Blending down");
    //            MusicSource.volume -= smoothValue * Time.deltaTime;
    //        }
    //        else
    //        {
    //            Debug.Log("Hit lowest target");
    //            MusicSource.volume = 0.1f;
    //        }
    //    }
    //    else
    //    {
    //        if(MusicSource.volume <= 0.5f)
    //        {
    //            Debug.Log("Blending up");

    //            MusicSource.volume += smoothValue * Time.deltaTime;
    //        }
    //        else
    //        {
    //            Debug.Log("Hit highest target");
    //            MusicSource.volume = 0.5f;
    //        }
    //    }
    //}
}
