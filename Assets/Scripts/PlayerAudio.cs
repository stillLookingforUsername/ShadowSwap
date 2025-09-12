using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    //private AudioSource audioSource;

    [Header("Clips")]
    public AudioClip jumpClip;
    public AudioClip walkClip;

    private bool isWalking = false;
    private float walkStepDelay = 0.4f; //time between footstep
    private float walktimer = 0f;
/*
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

*/
    public void PlayJump()
    {
        //audioSource.PlayOneShot(jumpClip);
        if(jumpClip != null)
            SFXManager.Instance.PlaySFX(jumpClip);
    }
    public void HandleWalkSound(bool walking)
    {
        if (isWalking)
        {
            walktimer -= Time.deltaTime;

            if (walktimer <= 0f)
            {
                //audioSource.PlayOneShot(walkClip);
                if(walkClip != null)
                    SFXManager.Instance.PlaySFX(walkClip);
                walktimer = walkStepDelay;
            }
        }
        else
        {
            walktimer = 0f;
        }
    }
}