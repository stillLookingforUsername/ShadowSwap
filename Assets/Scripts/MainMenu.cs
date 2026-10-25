using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string firstLevelName = "Level 1";
    public void PlayGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        if (SceneNavigation.IsLoading) return;
        if (SceneNavigation.Load(firstLevelName) && GameManager.Instance) GameManager.Instance.StartNewGame();
    }
    public void QuiteGame()
    {
        Application.Quit();
    }
}
