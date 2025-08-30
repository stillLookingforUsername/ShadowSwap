using System;
using UnityEngine;

public class RisingLavaTrigger : MonoBehaviour
{
   public RisingLava lava;  // Assign in Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement2D palyer))
        {
            lava.StartRising();
        }
    } 
}