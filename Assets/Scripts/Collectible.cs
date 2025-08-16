using UnityEngine;

public class Collectible : MonoBehaviour
{
    /*
    public static int Collected;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Collected++;
        Destroy(gameObject);
    }
    */
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

}