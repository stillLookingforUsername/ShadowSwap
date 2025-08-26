using UnityEngine;

public class LaneButton : MonoBehaviour
{
    //this script is to activate sth in the other lane 
    //( a button to activate sth in other lane - like platforms,anyGameObject,MovingPlatforms,etc)

    public MovingPlatform targetPlatformObject; // the object in the other lane what u want to activate or deactivate

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerMovement2D>())
        {
            Debug.Log("Player detect");
            if (targetPlatformObject != null)
            {
                Debug.Log("not null");
                targetPlatformObject.ActivatePlatform();
            }
        }
    }
}