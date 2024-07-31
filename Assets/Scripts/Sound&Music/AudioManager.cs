using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public float VolumeBoost = 1;

    public AudioSO sfx;
    public AudioSO bulks;
    public AudioSO steps;
    public AudioSO Music;

    public static AudioManager instance;
    public AudioMixer mixer;

    public bool muted = false;

    [SerializeField] private float _musicInterval;
    private int _currentMusic;
    private AudioSource _musicSource;
    private AudioSource _ambienceSource;

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
            sound.source = GenerateSource(sound);
        }
        foreach (Sound sound in bulks.Sounds)
        {
            sound.source = GenerateSource(sound);
        }
        foreach (Sound sound in steps.Sounds)
        {
            sound.source = GenerateSource(sound);
        }

        _musicSource = gameObject.AddComponent<AudioSource>();

        _ambienceSource = GenerateSource(Music.Sounds[0]);
        _ambienceSource.Play();

    }
    public void Start()
    {
        PlayNextMusic();
        SetAmbience(false);
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
        float soundLevel = Mathf.Lerp(0.001f, 1, volume / 100);
        mixer.SetFloat("SFXVolume", Mathf.Log(soundLevel) * 20);
    }
    public void SetBGMVolume(float volume)
    {

        float soundLevel = Mathf.Lerp(0.001f, 1, volume/100);
        mixer.SetFloat("BGMVolume", Mathf.Log(soundLevel) * 20);
    }

    private void PlayNextMusic()
    {
        int index;
        do
        {
            index = UnityEngine.Random.Range(1, Music.Sounds.Length);
        } while (index == _currentMusic);

        _currentMusic = index;

        var sound = Music.Sounds[index];
        _musicSource.clip = sound.clip;
        _musicSource.volume = sound.volume * VolumeBoost;
        _musicSource.pitch = sound.pitch;
        _musicSource.loop = sound.loop;
        _musicSource.outputAudioMixerGroup = sound.mixerGroup;
        _musicSource.Play();

        var duration = sound.clip.length;
        Utility.Delay(duration + _musicInterval, PlayNextMusic);
    }

    private AudioSource GenerateSource(Sound sound)
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.clip = sound.clip;

        source.volume = sound.volume * VolumeBoost;
        source.pitch = sound.pitch;
        source.loop = sound.loop;
        source.outputAudioMixerGroup = sound.mixerGroup;
        return source;
    }

    public void SetAmbience(bool ambience)
    {
        float music = ambience ? Mathf.Lerp(0.001f, 1, 0.7f) : 1;
        float ambient = ambience ? 1 : 0.001f;

        mixer.SetFloat("MusicVolume", Mathf.Log(music)*20);
        mixer.SetFloat("AmbienceVolume", Mathf.Log(ambient)*20);
    }
}