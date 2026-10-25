using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    private PlayerMovement2D playerMovement2D;

    // Event so UI (or other systems) can react to health changes
    public event Action<int, int> OnHealthChanged;
    private int lastDeathFrame = -1;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        Instance = this;
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = maxHealth;
        playerMovement2D = GetComponent<PlayerMovement2D>();
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || Time.timeScale <= 0f || LevelRunTracker.HasOpenResults || SceneNavigation.IsLoading || lastDeathFrame == Time.frameCount) return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
            return;
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void Die()
    {
        if (Time.timeScale <= 0f || LevelRunTracker.HasOpenResults || SceneNavigation.IsLoading || lastDeathFrame == Time.frameCount) return;
        lastDeathFrame = Time.frameCount;
        currentHealth = maxHealth;
        if (playerMovement2D) playerMovement2D.Respawn();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }


}
