using UnityEngine;

public class OrbAudio : MonoBehaviour
{
    [Header("Spatial Audio")]
    public Transform player;
    public float minDistance = 1f;
    public float maxDistance = 5f;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (!_audioSource) return;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private void Update()
    {
        if (!player && PlayerMovement2D.Instance) player = PlayerMovement2D.Instance.transform;
        if (!player || !_audioSource) return;

        float distance = Vector2.Distance(transform.position, player.position);

        //clamp between 0 and 1
        float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
        _audioSource.volume = Mathf.Clamp01(t) * (SFXManager.Instance ? SFXManager.Instance.Volume :
            Mathf.Clamp01(PlayerPrefs.GetFloat("SFXVolume", 1f)));
    }
}
