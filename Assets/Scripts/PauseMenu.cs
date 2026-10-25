using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseUI;
    private bool pause;

    private void OnEnable() => MobileGameInput.ActionPressed += OnTouchAction;
    private void OnDisable() => MobileGameInput.ActionPressed -= OnTouchAction;

    private void OnTouchAction(MobileGameInput.Control control)
    {
        if (control == MobileGameInput.Control.Pause) TooglePause();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TooglePause();
        }
    }
    public void TooglePause()
    {
        SetPaused(!pause);
    }

    public void SetPaused(bool paused)
    {
        // The completion screen owns pause until a navigation button is chosen.
        if (LevelRunTracker.HasOpenResults || SceneNavigation.IsLoading) return;
        pause = paused;
        MobileGameInput.Clear();
        if (PauseUI != null) PauseUI.SetActive(pause);
        Time.timeScale = pause ? 0 : 1;
    }

    public void Restart()
    {
        //make sure it's unpaused when restart
        Time.timeScale = 1;
        pause = false;
        MobileGameInput.Clear();

        if (PauseUI != null) PauseUI.SetActive(false);
        //UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        SceneNavigation.ReloadCurrent();
    }
}
