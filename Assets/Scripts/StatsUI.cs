using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCountTextMesh;

    private void Update()
    {
        UpdateTextMesh();
    }

    private void UpdateTextMesh()
    {
        coinCountTextMesh.text = GameManager.Instance.GetScore().ToString();
    }
}
