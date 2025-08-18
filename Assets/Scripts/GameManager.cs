using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int score = 0;
    public int requiredScore = 3;
    public ExitDoor exitDoor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        PlayerMovement2D.Instance.OnCoinPickUp += PlayerMovement2D_OnCoinPickUp;
    }

    private void PlayerMovement2D_OnCoinPickUp(object sender, EventArgs e)
    {
        AddScore(1);
    }

    private void AddScore(int scoreAmt)
    {
        score += scoreAmt;
        Debug.Log("Score: " + score);

        if (score >= requiredScore && exitDoor != null)
        {
            Debug.Log("Sufficient Score");
            exitDoor.Open();
        }
    }
    public int GetScore()
    {
        return score;
    }
}