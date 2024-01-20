using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class AudioMixerVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] GameObject musicSlider;
    [SerializeField] GameObject sfxSlider;

    float volume_music;
    float volume_sfx;

    void Start()
    {
        musicSlider.SetActive(false);
        sfxSlider.SetActive(false);
        if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadVolume();
        }
        else 
        {
            SetMusicVolume();
            SetSFXVolumen();
        }
    }

    public void SetMusicVolume()
    {
        volume_music = musicSlider.GetComponent<Slider>().value;
        myMixer.SetFloat("Music", Mathf.Log10(volume_music) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume_music);
    }

    public void SetSFXVolumen()
    {
        volume_sfx = sfxSlider.GetComponent<Slider>().value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume_sfx) *20);
        PlayerPrefs.SetFloat("sfxVolume", volume_sfx);
    }

    private void LoadVolume()
    {
        musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("sfxVolume");
        SetMusicVolume();
        SetSFXVolumen();
    }

    public void SilderView()
    {
        if (musicSlider.activeInHierarchy == false)
        {
            musicSlider.SetActive(true);
            sfxSlider.SetActive(true);
        }
        else if (musicSlider.activeInHierarchy == true)
        {
            musicSlider.SetActive(false);
            sfxSlider.SetActive(false);
        }

    }
}

