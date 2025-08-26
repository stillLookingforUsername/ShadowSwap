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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //reset or reuse exit door if present in new scene
        //exitDoor = FindObjectByType<ExitDoor>();

        //hook into the new player event
        if (PlayerMovement2D.Instance != null)
        {
            PlayerMovement2D.Instance.OnCoinPickUp += PlayerMovement2D_OnCoinPickUp;
        }
    }

    private void PlayerMovement2D_OnCoinPickUp(object sender, EventArgs e)
    {
        AddScore(1);
    }

    private void AddScore(int scoreAmt)
    {
        score += scoreAmt;
        Debug.Log("Score: " + score);
    }
    public int GetScore()
    {
        return score;
    }
}