using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseUI;
    private bool pause;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TooglePause();
        }
    }
    public void TooglePause()
    {
        pause = !pause;
        PauseUI.SetActive(pause);
        Time.timeScale = pause ? 0 : 1;
    }

    public void Restart()
    {
        //make sure it's unpaused when restart
        Time.timeScale = 1;
        pause = false;

        if (PauseUI != null) PauseUI.SetActive(false);
        //UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}