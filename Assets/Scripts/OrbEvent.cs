using UnityEngine;

public class OrbEvent : MonoBehaviour
{
    public event System.Action OnOrbDetectedPlayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<PlayerMovement2D>(out PlayerMovement2D player))
        {
            OnOrbDetectedPlayer?.Invoke();
        }
    }
}