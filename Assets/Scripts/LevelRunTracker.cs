using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerMovement2D))]
public class LevelRunTracker : MonoBehaviour
{
    [Tooltip("Optional permanent record ID; otherwise the scene asset path is used.")]
    [SerializeField] private string levelId;
    [Min(1)] public int airSwapGoal = 3;
    public static LevelRunTracker Instance { get; private set; }
    public static bool HasOpenResults => Instance && Instance.ResultsOpen;
    public LevelAttempt Attempt { get; private set; }
    public bool ResultsOpen { get; private set; }
    public LevelMasteryCompletion Completion { get; private set; }
    public int NextSceneIndex { get; private set; }
    public string LevelId => !string.IsNullOrWhiteSpace(levelId) ? levelId :
        !string.IsNullOrEmpty(gameObject.scene.path) ? gameObject.scene.path : gameObject.scene.name;

    private PlayerMovement2D player;
    private ShadowSwap swap;
    private bool transitioning;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset() => Instance = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only the player in the loaded gameplay scene receives an attempt.
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            var movement = root.GetComponentInChildren<PlayerMovement2D>();
            if (!movement) continue;
            var tracker = movement.GetComponent<LevelRunTracker>();
            if (!tracker) tracker = movement.gameObject.AddComponent<LevelRunTracker>();
            if (!FindFirstObjectByType<LevelMasteryUI>(FindObjectsInactive.Include)) LevelMasteryUI.Create(tracker);
            return;
        }
    }

    private void Awake()
    {
        Instance = this;
        player = GetComponent<PlayerMovement2D>();
        swap = GetComponent<ShadowSwap>();
        var pickupIds = new List<int>();
        foreach (GameObject root in gameObject.scene.GetRootGameObjects())
            foreach (Collectible collectible in root.GetComponentsInChildren<Collectible>(true))
                pickupIds.Add(collectible.GetInstanceID());
        Attempt = new LevelAttempt(pickupIds, airSwapGoal);
    }

    private void OnEnable()
    {
        if (player)
        {
            player.OnCollectiblePickedUp += OnPickup;
            player.OnPlayerRespawn += OnRespawn;
        }
        if (swap) swap.OnSwapAttempted += OnSwap;
    }

    private void OnDisable()
    {
        if (player)
        {
            player.OnCollectiblePickedUp -= OnPickup;
            player.OnPlayerRespawn -= OnRespawn;
        }
        if (swap) swap.OnSwapAttempted -= OnSwap;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update() => Attempt?.Advance(Time.deltaTime);
    private void OnPickup(Collectible collectible)
    {
        if (collectible) Attempt.Collect(collectible.GetInstanceID());
    }
    private void OnRespawn(object sender, EventArgs args) => Attempt.RecordDeath();
    private void OnSwap(object sender, ShadowSwap.SwapAttemptEventArgs args)
    {
        if (args.Outcome == ShadowSwap.SwapOutcome.Succeeded) Attempt.RecordSwap(args.WasAirborne);
    }

    public bool CompleteLevel(int nextSceneIndex)
    {
        if (!isActiveAndEnabled || Attempt == null || Attempt.IsComplete || Time.timeScale <= 0f) return false;
        Completion = LevelMasterySave.SaveCompletion(LevelId, Attempt.Complete());
        NextSceneIndex = nextSceneIndex;
        ResultsOpen = true;
        MobileGameInput.Clear();
        Time.timeScale = 0f;
        var ui = FindFirstObjectByType<LevelMasteryUI>(FindObjectsInactive.Include);
        if (!ui) ui = LevelMasteryUI.Create(this);
        ui.gameObject.SetActive(true);
        ui.ShowResults(Completion);
        return true;
    }

    public bool Continue() => Navigate(NextSceneIndex);
    public bool Replay() => Navigate(gameObject.scene.buildIndex);
    public bool MainMenu() => Navigate(MainMenuIndex());

    public static int MainMenuIndex()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            if (Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)) == "MainMenu") return i;
        return -1;
    }

    private bool Navigate(int sceneIndex)
    {
        if (!ResultsOpen || transitioning || sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings) return false;
        if (!SceneNavigation.Load(sceneIndex)) return false;
        transitioning = true;
        ResultsOpen = false;
        MobileGameInput.Clear();
        Time.timeScale = 1f;
        return true;
    }
}
