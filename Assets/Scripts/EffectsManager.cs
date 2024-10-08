using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance { get; private set; }

    public AudioSource backgroundMusicSource;  // Backgound music AudioSource
    public AudioSource sfxSource;              // Sound effect AudioSource

    [SerializeField] public AudioClip[] musicClips;             // Save all the Backgound music
    [SerializeField] public AudioClip[] sfxClips;               // Save all the sound effect


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBackgroundMusic(int clipIndex)
    {
        //if (clipIndex >= 0 && clipIndex < musicClips.Length)
        //{
        //    backgroundMusicSource.clip = musicClips[clipIndex];
        //    backgroundMusicSource.loop = true; 
        //    backgroundMusicSource.Play();
        //}
    }

    public void PlaySFX(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < sfxClips.Length)
        {
            sfxSource.PlayOneShot(sfxClips[clipIndex]);
        }
    }

    public void StopBackgroundMusic()
    {
        backgroundMusicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        backgroundMusicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

}