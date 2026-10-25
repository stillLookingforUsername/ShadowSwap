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
            if (Time.timeScale <= 0f || LevelRunTracker.HasOpenResults) return;
            var state = rb.GetComponent<GravityFlipState>();
            if (!state) state = rb.gameObject.AddComponent<GravityFlipState>();
            state.Flip(flipDuration);
            OnDeathZoneToogle?.Invoke(enableOnTrigger); //send mssg to enable/disable death trigger
        }
    }

}
