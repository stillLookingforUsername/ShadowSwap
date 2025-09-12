/*using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
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
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void PlaySFX(AudioClip clip, float volume = 1)
    {
        audioSource.PlayOneShot(clip, volume);
    }
    public void SetVolume(float volumeAmt)
    {
        audioSource.volume = volumeAmt;
    }
}
*/
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    private AudioSource audioSource;
    private float globalVolume = 1f; // new global sfx volume

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
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        audioSource.PlayOneShot(clip, volume * globalVolume);
    }

    public void SetVolume(float volumeAmt)
    {
        globalVolume = volumeAmt;
    }
}
