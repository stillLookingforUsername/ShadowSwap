using System;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShadowSwap : MonoBehaviour
{
    [Header("Shadow World")]
    public Vector2 shadowOffset = new Vector2(40f, 0f); // distance between lanes
    public Transform shadowGhost;                        // visual ghost
    public SpriteRenderer ghostRenderer;                 // to tint green/red
    public LayerMask blockingMask;                       // Ground_Over + Ground_Shadow + Hazards + NoSwap

    [Header("Safety Check")]
    public BoxCollider2D playerCollider;                 // reference on Player
    public float swapCooldown = 0.15f;

    [Header("UI Indicator")]
    public Image swapIndicatorUI;
    public Image cooldownUI;

    private bool _inShadow = false;
    private float _cooldown;
    public enum Lane { OverWorld, ShadowWorld }
    public Lane currentLane = Lane.OverWorld;

    public static event EventHandler<OnLaneChangedEventArgs> OnLaneChanged;
    public class OnLaneChangedEventArgs : EventArgs
    {
        public Lane NewLane { get; }
        public OnLaneChangedEventArgs(Lane newLane)
        {
            NewLane = newLane;
        }
    }

    private void Reset()
    {
        playerCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        _cooldown = Mathf.Max(0, _cooldown - Time.deltaTime);
        UpdateGhost();
        UpdateCooldownUI();
    }

    private void UpdateGhost()
    {
        Vector3 target = GetTargetPos();
        if (shadowGhost) shadowGhost.position = target;
        bool safe = IsSwapSafe(target);
        if (ghostRenderer) ghostRenderer.color = IsSwapSafe(target) ? Color.green : Color.red;
        if (swapIndicatorUI)
        {
            swapIndicatorUI.color = safe ? Color.green : Color.red;
        }
    }
    private void UpdateCooldownUI()
    {
        if (cooldownUI)
        {
            //normalize value : 1 = full , 0 = ready
            float t = 1f - (_cooldown / swapCooldown);
            cooldownUI.fillAmount = t;
        }
    }

    private Vector3 GetTargetPos()
    {
        return transform.position + (_inShadow ? (Vector3)(-shadowOffset) : (Vector3)shadowOffset);
    }

    private bool IsSwapSafe(Vector3 targetPos)
    {
        if (!playerCollider) return true;
        Vector2 size = playerCollider.bounds.size * 0.98f; // tiny shrink to avoid false positives
        return !Physics2D.OverlapBox(targetPos, size, 0f, blockingMask);
    }

    public void OnSwap(InputValue v)
    {
        if (!v.isPressed || _cooldown > 0f) return;

        Vector3 target = GetTargetPos();
        if (!IsSwapSafe(target)) return;

        transform.position = target;
        _inShadow = !_inShadow;
        _cooldown = swapCooldown;
        currentLane = (currentLane == Lane.OverWorld) ? Lane.ShadowWorld : Lane.OverWorld;

        OnLaneChanged?.Invoke(this, new OnLaneChangedEventArgs(currentLane));

        // Optional: tiny hitstop for juice
        // StartCoroutine(Hitstop(0.05f));
    }

    void OnDrawGizmosSelected()
    {
        if (!playerCollider) return;
        Vector3 target = GetTargetPos();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(target, playerCollider.bounds.size * 0.98f);
    }
}
