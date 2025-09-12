using UnityEngine;
using UnityEngine.SceneManagement;
public class DeathCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerHealth player))
        {
            if (player != null)
            {
                //player.Die();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}