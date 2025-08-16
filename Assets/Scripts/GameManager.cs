using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int score;

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
        AddScore(10);
    }

    private void AddScore(int scoreAmt)
    {
        score += scoreAmt;
        Debug.Log("Score: " + score);
    }
}