using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class AudioSettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    private void OnEnable()
    {
        //Load saved Settings
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSlider) musicSlider.SetValueWithoutNotify(musicVolume);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(sfxVolume);

        ApplyMusicVolume(musicVolume);
        ApplySFXVolume(sfxVolume);

        if (musicSlider) musicSlider.onValueChanged.AddListener(ApplyMusicVolume);
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(ApplySFXVolume);
    }
    private void ApplyMusicVolume(float musicVol)
    {
        musicVol = Mathf.Clamp01(musicVol);
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(musicVol);
        }
        PlayerPrefs.SetFloat("MusicVolume", musicVol);
    }
    private void ApplySFXVolume(float sfxVol)
    {
        sfxVol = Mathf.Clamp01(sfxVol);
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.SetVolume(sfxVol);
        }
        PlayerPrefs.SetFloat("SFXVolume", sfxVol);
    }
    private void OnDisable()
    {
        if (musicSlider) musicSlider.onValueChanged.RemoveListener(ApplyMusicVolume);
        if (sfxSlider) sfxSlider.onValueChanged.RemoveListener(ApplySFXVolume);
        PlayerPrefs.Save();
    }
    private void OnApplicationPause(bool paused) { if (paused) PlayerPrefs.Save(); }
}
