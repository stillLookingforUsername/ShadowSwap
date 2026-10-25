using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCountTextMesh;
    private int displayedScore = int.MinValue;

    private void Update()
    {
        UpdateTextMesh();
    }

    private void UpdateTextMesh()
    {
        if (!coinCountTextMesh) return;
        int score = GameManager.Instance ? GameManager.Instance.GetScore() : 0;
        if (score == displayedScore) return;
        displayedScore = score;
        coinCountTextMesh.text = score.ToString();
    }
}
