using UnityEngine;
using UnityEngine.SceneManagement;

public class EnableDeathTrigger : MonoBehaviour
{
    [SerializeField] private bool isActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;
        if (other.TryGetComponent<PlayerMovement2D>(out PlayerMovement2D player))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void EnableZone()
    {
        isActive = true;
    }
    public void DisableZone()
    {
        isActive = false;
    }

    private void OnEnable()
    {
        GravityFlipTrap.OnDeathZoneToogle += HandleDeathZoneTrigger;
        DisableDeathTrigger.OnDisableZone += HandleDeathZoneTrigger;
    }
    private void OnDisable()
    {
        GravityFlipTrap.OnDeathZoneToogle -= HandleDeathZoneTrigger;
        DisableDeathTrigger.OnDisableZone -= HandleDeathZoneTrigger;
    }

    private void HandleDeathZoneTrigger(bool enable)
    {
        if(enable)
        {
            EnableZone();
        }
        else
        {
            DisableZone();
        }
    }

}