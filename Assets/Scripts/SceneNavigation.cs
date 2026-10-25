using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNavigation
{
    public static bool IsLoading { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset() => IsLoading = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded -= Loaded;
        SceneManager.sceneLoaded += Loaded;
    }

    private static void Loaded(Scene scene, LoadSceneMode mode) => IsLoading = false;

    public static bool Load(int index)
    {
        if (IsLoading || index < 0 || index >= SceneManager.sceneCountInBuildSettings) return false;
        IsLoading = true;
        MobileGameInput.Clear();
        Time.timeScale = 1f;
        SceneManager.LoadScene(index);
        return true;
    }

    public static bool Load(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            if (Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)) == sceneName) return Load(i);
        return false;
    }

    public static bool ReloadCurrent() => Load(SceneManager.GetActiveScene().buildIndex);
}
