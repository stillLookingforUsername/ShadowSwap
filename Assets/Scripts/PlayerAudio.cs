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

    public void PlayJump()
    {
        //audioSource.PlayOneShot(jumpClip);
        if(jumpClip != null && SFXManager.Instance)
            SFXManager.Instance.PlaySFX(jumpClip);
    }
    public void HandleWalkSound(bool walking)
    {
        isWalking = walking;
        if (isWalking)
        {
            walktimer -= Time.deltaTime;

            if (walktimer <= 0f)
            {
                //audioSource.PlayOneShot(walkClip);
                if(walkClip != null && SFXManager.Instance)
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
