using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
/// <summary>
/// Four built-in functions. Play, Stop, Pause and Unpause. Requires the string of the target sound clip for all.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer masterMixer;

    public Sound[] sounds;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        GameObject audioSources = new GameObject { name = "Audio Sources" };
        audioSources.transform.SetParent(transform);
        foreach (var s in sounds)
        {
            s.source = audioSources.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = s.mixerGroup;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.mute = s.mute;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;

        }

        DontDestroyOnLoad(gameObject);
    }

    //public void PlayAudio(int i)
    //{
    //    source.enabled = true;
    //    switch (i)
    //    {
    //        case 0:
    //            source.clip = clips[i];
    //            source.Play();
    //            break;
    //        case 1:
    //            source.clip = clips[i];
    //            source.Play();
    //            break;
    //        case 2:
    //            source.clip = clips[i];
    //            source.Play();
    //            break;
    //        case 3:
    //            source.clip = clips[i];
    //            source.Play();
    //            break;
    //        case 4:
    //            source.clip = clips[i];
    //            source.Play();
    //            break;
    //        case 5:
    //            source.clip = clips[i];
    //            source.Play();
    //            break;
    //    }
    //}
    //
    /// <summary>
    /// <paramref name="soundName"/> is case sensitive!
    /// </summary>
    /// <param name="soundName"></param>
    public void Play(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.Log($"The sound called ''{soundName}'' is not found and cannot be played."); return;
        }

        if (s.clipVariants.Length > 1)
        {
            s.clip = s.clipVariants[UnityEngine.Random.Range(0, s.clipVariants.Length)];
            s.source.clip = s.clip;
        }

       
        s.source.Play();
    }

    /// <summary>
    /// <paramref name="soundName"/> is case sensitive!
    /// </summary>
    /// <param name="soundName"></param>
    public void Stop(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.Log($"The sound called ''{soundName}'' is not found and cannot be stopped."); return;
        }
        s.source.Stop();
    }

    /// <summary>
    /// <paramref name="soundName"/> is case sensitive!
    /// </summary>
    /// <param name="soundName"></param>
    public void Pause(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.Log($"The sound called ''{soundName}'' is not found and cannot be paused."); return;
        }
        s.source.Pause();
    }

    /// <summary>
    /// <paramref name="soundName"/> is case sensitive!
    /// </summary>
    /// <param name="soundName"></param>
    public void Unpause(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.Log($"The sound called ''{soundName}'' is not found and cannot be unpaused."); return;
        }
        s.source.UnPause();
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public AudioClip[] clipVariants;
    public AudioMixerGroup mixerGroup;
    [Range(0, 1)] public float volume = 0.5f;
    [Range(0, 3)] public float pitch = 1f;
    public bool loop = false;
    public bool mute = false;
    public bool playOnAwake = false;
    [HideInInspector] public AudioSource source;
}