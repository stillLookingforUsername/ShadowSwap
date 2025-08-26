using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //If you don’t need the variable playerMovement2d other wise use (out PlayerMovement2D playerMovement2d);
        Debug.Log("Door detect player");
        //if (isOpen && other.gameObject.GetComponent<PlayerMovement2D>())
        if (other.gameObject.TryGetComponent<PlayerMovement2D>(out _))
        {
            Debug.Log("Level Completed");

            //Load next Level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            //Move the Door using Dotween to show the is opening,etc
        }
    }

    //IEnumerator to move the door (optional)
}


