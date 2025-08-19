using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth; // drag in Inspector
    [SerializeField] private Image[] healthImages;

    private void Awake()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < healthImages.Length; i++)
        {
            if (i < currentHealth)
                SetImageAlpha(healthImages[i], 1f); // full
            else
                SetImageAlpha(healthImages[i], 0.2f); // faded
        }
    }

    private void SetImageAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}