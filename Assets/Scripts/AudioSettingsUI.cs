using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class AudioSettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        //Load saved Settings
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        ApplyMusicVolume(musicVolume);
        ApplySFXVolume(sfxVolume);

        musicSlider.onValueChanged.AddListener(ApplyMusicVolume);
        sfxSlider.onValueChanged.AddListener(ApplySFXVolume);
    }
    private void ApplyMusicVolume(float musicVol)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(musicVol);
        }
        PlayerPrefs.SetFloat("MusicVolume", musicVol);
    }
    private void ApplySFXVolume(float sfxVol)
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.SetVolume(sfxVol);
        }
        PlayerPrefs.SetFloat("SFXVolume", sfxVol);
    }
}