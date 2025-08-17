using UnityEngine;

public class LaneButton : MonoBehaviour
{
    //this script is to activate sth in the other lane 
    //( a button to activate sth in other lane - like platforms,anyGameObject,MovingPlatforms,etc)

    public GameObject targetPlatformObject; // the object in the other lane what u want to activate or deactivate
    public bool toggleSprite = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerMovement2D>())
        {
            if (targetPlatformObject != null)
            {
                targetPlatformObject.SetActive(toggleSprite);
            }
        }
    }
}