using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushBox : MonoBehaviour
{
    private Rigidbody2D _rb;

    [Header("Push Settings")]
    public float pushForce = 5f; // how hard player pushes

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
   

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Vector2 pushDir = collision.GetContact(0).normal * -1; // opposite of collision normal
            _rb.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
        }
    }
}
