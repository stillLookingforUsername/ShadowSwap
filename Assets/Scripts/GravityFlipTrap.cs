using System;
using System.Collections;
using UnityEngine;
public class GravityFlipTrap : MonoBehaviour
{

    public static event Action<bool> OnDeathZoneToogle;
    [SerializeField] private bool enableOnTrigger = true;
    public float flipDuration = 3f; // How long gravity stays inverted

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Rigidbody2D rb))
        {
            StartCoroutine(FlipGravity(rb));
            OnDeathZoneToogle?.Invoke(enableOnTrigger); //send mssg to enable/disable death trigger
        }
    }

    private IEnumerator FlipGravity(Rigidbody2D rb)
    {
        Debug.Log("Gravity flipped!");

        // Invert player gravity
        rb.gravityScale *= -1;

        // Flip player visually (optional: upside-down sprite)
        rb.transform.localScale = new Vector3(rb.transform.localScale.x, -rb.transform.localScale.y, rb.transform.localScale.z);

        // Wait for duration
        yield return new WaitForSeconds(flipDuration);

        // Restore gravity
        rb.gravityScale *= -1;

        //to effect everything thing
        //Physics2D.gravity *= -1;


        // Flip sprite back
        rb.transform.localScale = new Vector3(rb.transform.localScale.x, -rb.transform.localScale.y, rb.transform.localScale.z);

        Debug.Log("Gravity restored!");
    }
}