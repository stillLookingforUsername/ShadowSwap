using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true;
        audioSource.playOnAwake = true;
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (audioSource.clip == clip) return;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }

    public void SetVolume(float volumeAmt)
    {
        audioSource.volume = volumeAmt;
    }

}