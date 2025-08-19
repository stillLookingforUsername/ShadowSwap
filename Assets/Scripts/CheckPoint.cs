using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement2D playerMovement2D))
        {
            if (playerMovement2D != null)
            {
                playerMovement2D.SetRespawn(transform.position);
            }
        }
    }
}