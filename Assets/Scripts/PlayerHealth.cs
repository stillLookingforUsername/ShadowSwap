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
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        playerMovement2D = GetComponent<PlayerMovement2D>();
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("TakeDamge called");
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("Player died");
            Die();
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void Die()
    {
        currentHealth = maxHealth;
        playerMovement2D.Respawn();
    }

/*
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    */
}
