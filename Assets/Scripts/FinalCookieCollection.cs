using TMPro;
using UnityEngine;
public class FinalCookieCollection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalCookieText;
    private int displayedScore = int.MinValue;

    private void Update()
    {
        if (!finalCookieText) return;
        int score = GameManager.Instance ? GameManager.Instance.GetScore() : 0;
        if (score == displayedScore) return;
        displayedScore = score;
        finalCookieText.text = "Cookies : " + score;
    }
}
