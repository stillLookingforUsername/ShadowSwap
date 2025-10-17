using System;
using UnityEngine;

public class DisableDeathTrigger : MonoBehaviour
{
    public static event Action<bool> OnDisableZone;
    [SerializeField] private bool disableOnTrigger = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<PlayerMovement2D>(out PlayerMovement2D player))
        {
            OnDisableZone?.Invoke(disableOnTrigger);
        }
    }

}