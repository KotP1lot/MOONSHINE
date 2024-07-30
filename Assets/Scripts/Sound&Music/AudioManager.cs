using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public float VolumeBoost = 1;

    public AudioSO sfx;
    public AudioSO bulks;
    public AudioSO steps;

    public static AudioManager instance;
    public AudioMixer mixer;

    public bool muted = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            return;
        }

        foreach (Sound sound in sfx.Sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume * VolumeBoost;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = sound.mixerGroup;
        }
        foreach (Sound sound in bulks.Sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume * VolumeBoost;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = sound.mixerGroup;
        }
        foreach (Sound sound in steps.Sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume * VolumeBoost;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = sound.mixerGroup;
        }
    }
    public void Start()
    {
        Play("Theme");
    }
    public void Play(string name)
    {
        if (!muted || name == "UIHover" || name == "UIClick")
        {
            Sound s = Array.Find(sfx.Sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.Log("Sound" + name + "not found");
                return;
            }
            s.source.Play();
        }
    }

    public void Bulk()
    {
        Sound s = bulks.Sounds[UnityEngine.Random.Range(0, bulks.Sounds.Length)];
        
        s.source.Play();
    }
    public void Step()
    {
        Sound s = steps.Sounds[UnityEngine.Random.Range(0, steps.Sounds.Length)];

        s.source.Play();
    }

    public void SetSFXVolume(float volume)
    {

    }
    public void SetBGMVolume(float volume)
    {

    }
}