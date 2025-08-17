using UnityEngine;

public class Collectible : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

}