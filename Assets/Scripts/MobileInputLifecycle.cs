using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Installs touch controls on phones and clears input after interruptions.</summary>
public class MobileInputLifecycle : MonoBehaviour
{
    [Tooltip("Rendering target on phones; physics timing is unchanged.")]
    [Range(30, 120)] public int mobileFrameRate = 60;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        var host = new GameObject("Mobile Input Lifecycle");
        DontDestroyOnLoad(host);
        host.AddComponent<MobileInputLifecycle>();
    }

    private void Awake()
    {
        if (Application.isMobilePlatform)
        {
            Application.targetFrameRate = mobileFrameRate;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MobileGameInput.Clear();
        if (!Application.isMobilePlatform) return;

        var player = FindFirstObjectByType<PlayerMovement2D>();
        if (!player || FindFirstObjectByType<MobileControlsUI>(FindObjectsInactive.Include)) return;
        // A handcrafted layout can use only MobileControlButton components.
        if (FindFirstObjectByType<MobileControlButton>(FindObjectsInactive.Include)) return;

        MobileControlsUI.CreateDefault();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) HandleInterruption();
    }

    private void OnApplicationPause(bool suspended)
    {
        if (suspended) HandleInterruption();
    }

    private void HandleInterruption()
    {
        MobileGameInput.Clear();
        if (!Application.isMobilePlatform) return;
        var pauseMenu = FindFirstObjectByType<PauseMenu>();
        if (pauseMenu) pauseMenu.SetPaused(true);
    }
}
