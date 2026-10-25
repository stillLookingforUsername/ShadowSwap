using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int score = 0;
    public int requiredScore = 3;
    public event Action<int> OnScoreChanged;
    private PlayerMovement2D player;
    private readonly System.Collections.Generic.HashSet<string> collectedCookies = new System.Collections.Generic.HashSet<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        if (!FindFirstObjectByType<GameManager>(FindObjectsInactive.Include)) new GameObject("GameManager").AddComponent<GameManager>();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
  

    private void OnEnable()
    {
        if (Instance != this) return;
        SceneManager.sceneLoaded += OnSceneLoaded;
        BindPlayer();
    }
    private void Start() { if (Instance == this) BindPlayer(); }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnbindPlayer();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //reset or reuse exit door if present in new scene
        //exitDoor = FindObjectByType<ExitDoor>();

        //hook into the new player event
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Collectible collectible in root.GetComponentsInChildren<Collectible>(true)) _ = collectible.CollectionId;
        BindPlayer();
    }

    private void BindPlayer()
    {
        UnbindPlayer();
        player = PlayerMovement2D.Instance;
        if (player) player.OnCollectiblePickedUp += OnPickup;
    }
    private void UnbindPlayer()
    {
        if (player) player.OnCollectiblePickedUp -= OnPickup;
        player = null;
    }
    private void OnPickup(Collectible collectible)
    {
        if (collectible && collectedCookies.Add(collectible.CollectionId)) AddScore(1);
    }
    public void StartNewGame()
    {
        collectedCookies.Clear();
        score = 0;
        OnScoreChanged?.Invoke(score);
    }
    private void OnDestroy()
    {
        UnbindPlayer();
        if (Instance == this) Instance = null;
    }

    private void AddScore(int scoreAmt)
    {
        score += scoreAmt;
        OnScoreChanged?.Invoke(score);
    }
    public int GetScore()
    {
        return score;
    }
}
