using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderSoundsChanger : MonoBehaviour
{
    public Slider sliderSounds, sliderMusic, slideMaster;
    public AudioMixerGroup mixerSounds, mixerMusic, mixerMaster;
    public float GetMusicLevel()
    {
        float value;
        bool result = mixerMusic.audioMixer.GetFloat("MusicVolume", out value);
        if (result)
        {
            return (value + 80) / 80;
        }
        else
        {
            return 0f;
        }
    }
    public float GetSoundsLevel()
    {
        float value;
        bool result = mixerSounds.audioMixer.GetFloat("SoundsVolume", out value);
        if (result)
        {
            return (value + 80) / 80;
        }
        else
        {
            return 0f;
        }
    }
    public float GetMasterLevel()
    {
        float value;
        bool result = mixerSounds.audioMixer.GetFloat("MasterVolume", out value);
        if (result)
        {
            return (value + 80) / 80;
        }
        else
        {
            return 0f;
        }
    }
    private void Start()
    {
        PauseController.pauseChanger += UpdateSlider;
        sliderSounds.value = GetSoundsLevel();
        sliderMusic.value = GetMusicLevel();
        slideMaster.value = GetMasterLevel();
    }
    public void UpdateSlider()
    {
        sliderSounds.value = GetSoundsLevel();
        sliderMusic.value = GetMusicLevel();
        slideMaster.value = GetMasterLevel();
    }

}
