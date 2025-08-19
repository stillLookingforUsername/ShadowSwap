using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    // Event so UI (or other systems) can react to health changes
    public event Action<int, int> OnHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("TakeDamge called");
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("Player died");
            
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
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
