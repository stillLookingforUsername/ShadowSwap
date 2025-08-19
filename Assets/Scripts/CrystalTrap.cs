using UnityEngine;

public class CrystalTrap : MonoBehaviour
{
    private int damage = 1;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
