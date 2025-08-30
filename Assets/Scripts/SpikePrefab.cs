using UnityEngine;

public class SpikePrefab : MonoBehaviour
{
     [Header("Spike Settings")]
    public float riseHeight = 1f;       // how high spikes rise
    public float riseSpeed = 5f;        // speed going up
    public float stayDuration = 2f;     // how long spikes stay up
    public float retractSpeed = 5f;     // speed going down

    private Vector3 startPos;
    private Vector3 targetPos;
    private float timer = 0f;
    private enum State { Rising, Staying, Retracting, Idle }
    private State state = State.Rising;

    private void Start()
    {
        startPos = transform.position;                  // starting below ground
        targetPos = startPos + Vector3.up * riseHeight; // target above ground
    }

    private void Update()
    {
        switch (state)
        {
            case State.Rising:
                transform.position = Vector3.MoveTowards(transform.position, targetPos, riseSpeed * Time.deltaTime);
                if (transform.position == targetPos)
                {
                    state = State.Staying;
                    timer = stayDuration;
                }
                break;

            case State.Staying:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = State.Retracting;
                }
                break;

            case State.Retracting:
                transform.position = Vector3.MoveTowards(transform.position, startPos, retractSpeed * Time.deltaTime);
                if (transform.position == startPos)
                {
                    state = State.Idle; // done, back underground
                }
                break;

            case State.Idle:
                // Optional: destroy here OR reset to Rising if you want looping traps
                // Destroy(gameObject);
                break;
        }
    }
}