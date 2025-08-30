using UnityEngine;

public class RetractingSpike : MonoBehaviour
{
     [Header("Spike Movement Settings")]
    public float upOffset = 1f;       // How far spikes rise up
    public float speed = 2f;          // How fast spikes move
    public float activeTime = 2f;     // How long spikes stay up
    public float inactiveTime = 2f;   // How long spikes stay down

    private Vector3 downPos;
    private Vector3 upPos;
    private bool isActive = false;
    private float timer;

    void Start()
    {
        // Store start pos as the down position
        downPos = transform.position;
        upPos = downPos + Vector3.up * upOffset;

        timer = inactiveTime; // start hidden
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (!isActive && timer <= 0f)
        {
            // Rise up
            isActive = true;
            timer = activeTime;
        }
        else if (isActive && timer <= 0f)
        {
            // Retract down
            isActive = false;
            timer = inactiveTime;
        }

        // Smooth movement between positions
        Vector3 targetPos = isActive ? upPos : downPos;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive && collision.gameObject.CompareTag("Player"))
        {
            // Example: kill player
            Debug.Log("Player hit by spikes!");
            // You can call your player damage function here
        }
    }
}