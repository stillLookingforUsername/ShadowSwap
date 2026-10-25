using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.timeScale <= 0f || SceneNavigation.IsLoading) return;
        //If you don’t need the variable playerMovement2d other wise use (out PlayerMovement2D playerMovement2d);
        Debug.Log("Door detect player");
        //if (isOpen && other.gameObject.GetComponent<PlayerMovement2D>())
        if (other.gameObject.TryGetComponent<PlayerMovement2D>(out _))
        {
            Debug.Log("Level Completed");

            int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
            var mastery = other.GetComponent<LevelRunTracker>();
            if (mastery && mastery.isActiveAndEnabled) mastery.CompleteLevel(nextScene);
            else SceneNavigation.Load(nextScene);
            //Move the Door using Dotween to show the is opening,etc
        }
    }

    //IEnumerator to move the door (optional)
}


