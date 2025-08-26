using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public float speed = 5f;

    private Vector3 nextPoint;

    private bool shouldMove = false; 
    private void Start()
    {
        nextPoint = pointA.position;
    }

    private void Update()
    {
        if (!shouldMove) return; // stop until activated

        transform.position = Vector3.MoveTowards(transform.position, nextPoint, speed * Time.deltaTime);
        
        if(transform.position == nextPoint)
        {
            nextPoint = (nextPoint == pointA.position) ? pointB.position : pointA.position;
        }
    }

    public void ActivatePlatform()
    {
        Debug.Log("is called");
        shouldMove = true;
    }
    public void DeActivatePlatform()
    {
        shouldMove = false;
    }

    //to set the player as a child of the platform
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerMovement2D player))
        {
            collision.transform.SetParent(transform);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerMovement2D player))
        {
            collision.transform.SetParent(null);
        }
    }
}
