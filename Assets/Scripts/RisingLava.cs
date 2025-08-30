using UnityEngine;

public class RisingLava : MonoBehaviour
{
    public float riseSpeed = 1f;   // Units per second

    private bool isRising = false;

    void Update()
    {
        if (isRising)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        }
    }

    public void StartRising()
    {
        isRising = true;
    }

    public void StopRising()
    {
        isRising = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerHealth player))
        {
            player.Die();
        }
    }

}