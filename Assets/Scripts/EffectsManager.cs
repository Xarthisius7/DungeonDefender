using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance { get; private set; }

    public AudioSource backgroundMusicSource;  // Backgound music AudioSource
    public AudioSource sfxSource;              // Sound effect AudioSource

    [SerializeField] public AudioClip[] musicClips; // Save all the Backgound music
    [SerializeField] public AudioClip[] sfxClips;   // Save all the sound effect

    public float globalSFXVolume = 1.0f;  // Global SFX volume, range from 0 to 1
    public float backgroundMusicVolume = 1.0f;  // Background music volume, range from 0 to 1
    public float fadeDuration = 1.5f; // Smooth out BGM duration

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
    private void Start()
    {
        // Set initial volumes
        sfxSource.volume = globalSFXVolume;
        backgroundMusicSource.volume = backgroundMusicVolume;

        PlayBackgroundMusic(0);
    }
    private void UpdateVolumeSetting()
    {
        // Update volumes based on public variables
        sfxSource.volume = globalSFXVolume;
        backgroundMusicSource.volume = backgroundMusicVolume;
    }



    public void StopBackgroundMusic()
    {
        backgroundMusicSource.Stop();
    }

    public void PlayBackgroundMusic(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < musicClips.Length)
        {
            backgroundMusicSource.clip = musicClips[clipIndex];
            backgroundMusicSource.Play();
        }
    }

    public void PlayBackgroundMusicSmooth(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < musicClips.Length)
        {
            StartCoroutine(FadeOutAndPlayNewClip(clipIndex));
        }
    }
    private System.Collections.IEnumerator FadeOutAndPlayNewClip(int clipIndex)
    {
        float startVolume = backgroundMusicSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            backgroundMusicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        backgroundMusicSource.Stop();

        // Switch to new bgm
        backgroundMusicSource.clip = musicClips[clipIndex];
        backgroundMusicSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            backgroundMusicSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        // turning back the volume.
        backgroundMusicSource.volume = startVolume;
    }


    public void PauseBackgroundMusic()
    {
        backgroundMusicSource.Pause();
    }

    public void ResumeBackgroundMusic()
    {
        if (!backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Play();
        }
    }



    public void PlaySFX(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < sfxClips.Length)
        {
            sfxSource.PlayOneShot(sfxClips[clipIndex], globalSFXVolume);
        }
    }

    public void PlaySFX(int clipIndex, float volume = 1.0f)
    {
        if (clipIndex >= 0 && clipIndex < sfxClips.Length)
        {
            // Ensure volume is clamped between 0.0 and 1.0
            volume = Mathf.Clamp(volume, 0.0f, 1.0f);
            sfxSource.PlayOneShot(sfxClips[clipIndex], volume * globalSFXVolume);
        }
    }




}