using UnityEngine;

public class Patroller2D : MonoBehaviour
{
    public Transform a, b;
    public float speed = 2f;
    Transform _target;
    void Start(){ _target = b; }
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, _target.position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, _target.position) < 0.05f)
            _target = _target == a ? b : a;
    }
    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.CompareTag("Player"))
        {
            // TODO: call your respawn
        }
    }
}
