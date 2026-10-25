using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    private AudioSource audioSource;
    private float globalVolume = 1f; // new global sfx volume
    public float Volume => globalVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 1f;
        globalVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("SFXVolume", 1f));
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (audioSource && clip) audioSource.PlayOneShot(clip, Mathf.Clamp01(volume) * globalVolume);
    }

    public void SetVolume(float volumeAmt)
    {
        globalVolume = Mathf.Clamp01(volumeAmt);
    }
    private void OnDestroy() { if (Instance == this) Instance = null; }
}
