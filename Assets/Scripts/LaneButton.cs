using UnityEngine;

public class LaneButton : MonoBehaviour
{
    //this script is to activate sth in the other lane 
    //( a button to activate sth in other lane - like platforms,anyGameObject,MovingPlatforms,etc)

    [Tooltip("Assign a MovingPlatform component or the GameObject containing it.")]
    public UnityEngine.Object targetPlatformObject;

    // Older scenes serialized the GameObject; accept both without changing scene files.
    public MovingPlatform TargetPlatform => targetPlatformObject is MovingPlatform platform ? platform :
        targetPlatformObject is GameObject target ? target.GetComponent<MovingPlatform>() : null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerMovement2D>())
        {
            Debug.Log("Player detect");
            var platform = TargetPlatform;
            if (platform != null)
            {
                Debug.Log("not null");
                platform.ActivatePlatform();
            }
        }
    }
}
