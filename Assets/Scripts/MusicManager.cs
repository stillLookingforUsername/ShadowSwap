using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private AudioSource audioSource;

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
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = Mathf.Clamp01(PlayerPrefs.GetFloat("MusicVolume", 1f));
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (!audioSource || !clip) return;
        if (audioSource.clip == clip && audioSource.isPlaying) return;
        audioSource.clip = clip;
        audioSource.volume = Mathf.Clamp01(PlayerPrefs.GetFloat("MusicVolume", 1f)) * Mathf.Clamp01(volume);
        audioSource.Play();
    }

    public void SetVolume(float volumeAmt)
    {
        if (audioSource) audioSource.volume = Mathf.Clamp01(volumeAmt);
    }

    private void OnDestroy() { if (Instance == this) Instance = null; }

}
