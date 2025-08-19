using UnityEngine;

public class EnemyPatroller2D : MonoBehaviour
{
    public Transform a, b;
    public float speed = 2f;
    private int damage = 1;
    private Transform _target;
    private void Start(){ _target = b; }
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, _target.position, speed * Time.deltaTime);

         // Flip based on direction
        if (_target.position.x > transform.position.x)
        {
            // moving right
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            // moving left
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        if (Vector2.Distance(transform.position, _target.position) < 0.05f)
        {
            _target = _target == a ? b : a;
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerHealth playerHealth))
        {
            Debug.Log("player touch");
            playerHealth.TakeDamage(damage);
        }
    }
}
