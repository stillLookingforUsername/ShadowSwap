using System;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ShadowSwap : MonoBehaviour
{
    [Header("Shadow World")]
    public Vector2 shadowOffset = new Vector2(150f, 0f); // distance between lanes
    public Transform shadowGhost;                        // visual ghost
    public SpriteRenderer ghostRenderer;                 // to tint green/red
    public LayerMask blockingMask;                       // Ground_Over + Ground_Shadow + Hazards + NoSwap

    [Header("Safety Check")]
    public BoxCollider2D playerCollider;                 // reference on Player
    public float swapCooldown = 0.15f;

    [Header("Shadow Timer")]
    public float shadowTimeLimit = 5f;
    private float shadowTimer;

    [Header("UI Indicator")]
    public Image swapIndicatorUI;
    public Image cooldownUI;

    private bool _inShadow = false;
    private float _cooldown;
    private Rigidbody2D _rb;
    private PlayerMovement2D _movement;
    public enum Lane { OverWorld, ShadowWorld }
    public Lane currentLane = Lane.OverWorld;
    [Header("TimerUI")]
    public TMPro.TextMeshProUGUI shadowTimerUI;

    public static event EventHandler<OnLaneChangedEventArgs> OnLaneChanged;
    public enum SwapOutcome { Succeeded, Blocked, CoolingDown }
    public event EventHandler<SwapAttemptEventArgs> OnSwapAttempted;
    public bool IsInShadow => _inShadow;
    public bool DestinationClear { get; private set; }
    public float CooldownRemaining => _cooldown;
    public float ShadowTimeRemaining => _inShadow ? Mathf.Max(0f, shadowTimer) : shadowTimeLimit;

    public class SwapAttemptEventArgs : EventArgs
    {
        public SwapOutcome Outcome { get; }
        public Vector3 Origin { get; }
        public Vector3 Destination { get; }
        public bool WasAirborne { get; }

        public SwapAttemptEventArgs(SwapOutcome outcome, Vector3 origin, Vector3 destination, bool wasAirborne = false)
        {
            Outcome = outcome;
            Origin = origin;
            Destination = destination;
            WasAirborne = wasAirborne;
        }
    }
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

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _movement = GetComponent<PlayerMovement2D>();
        if (!playerCollider) playerCollider = GetComponent<BoxCollider2D>();
        blockingMask |= LayerMask.GetMask("Hazard_Over", "Hazard_Shadow");
        shadowTimer = shadowTimeLimit;
    }

    private void OnEnable() => MobileGameInput.ActionPressed += OnTouchAction;
    private void OnDisable() => MobileGameInput.ActionPressed -= OnTouchAction;

    private void OnTouchAction(MobileGameInput.Control control)
    {
        if (control == MobileGameInput.Control.Swap) RequestSwap();
    }

    private void Update()
    {
        _cooldown = Mathf.Max(0, _cooldown - Time.deltaTime);
        UpdateGhost();
        UpdateCooldownUI();
        UpdateShadowTimer();
    }

    private void UpdateShadowTimer()
    {
        if (_inShadow)
        {
            shadowTimer -= Time.deltaTime;
            if (shadowTimer <= 0f)
            {
                Debug.Log("Player Dies");
                //PlayerHealth.Instance.Die();
                SceneNavigation.ReloadCurrent();
            }
        }
        else
        {
            //reset in Overworld
            shadowTimer = shadowTimeLimit;
        }
        if (shadowTimerUI)
        {
            shadowTimerUI.text = shadowTimer.ToString("0.0");
        }

    }
    private void UpdateGhost()
    {
        Vector3 target = GetTargetPos();
        if (shadowGhost) shadowGhost.position = transform.position + LaneOffset();
        bool safe = IsSwapSafe(target);
        DestinationClear = safe;
        if (ghostRenderer) ghostRenderer.color = safe ? Color.green : Color.red;
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
            float t = swapCooldown > 0f ? Mathf.Clamp01(1f - (_cooldown / swapCooldown)) : 1f;
            cooldownUI.fillAmount = t;
        }
    }

    private Vector3 GetTargetPos()
    {
        Vector3 position = _rb ? new Vector3(_rb.position.x, _rb.position.y, transform.position.z) : transform.position;
        return position + LaneOffset();
    }

    private Vector3 LaneOffset() => _inShadow ? (Vector3)(-shadowOffset) : (Vector3)shadowOffset;

    private bool IsSwapSafe(Vector3 targetPos)
    {
        if (!playerCollider) return false;
        Vector2 size = playerCollider.bounds.size * 0.98f; // tiny shrink to avoid false positives
        Vector2 origin = _rb ? _rb.position : (Vector2)transform.position;
        Vector2 center = (Vector2)playerCollider.bounds.center + (Vector2)targetPos - origin;
        return !Physics2D.OverlapBox(center, size, 0f, blockingMask);
    }

    public void OnSwap(InputValue v)
    {
        if (v.isPressed) RequestSwap();
    }

    public void RequestSwap()
    {
        if (!isActiveAndEnabled || Time.timeScale <= 0f || SceneNavigation.IsLoading ||
            (_movement && !_movement.CanAcceptInput)) return;

        Vector3 target = GetTargetPos();
        Vector3 origin = target - LaneOffset();
        if (_cooldown > 0f)
        {
            OnSwapAttempted?.Invoke(this, new SwapAttemptEventArgs(SwapOutcome.CoolingDown, origin, target));
            return;
        }
        if (!IsSwapSafe(target))
        {
            OnSwapAttempted?.Invoke(this, new SwapAttemptEventArgs(SwapOutcome.Blocked, origin, target));
            return;
        }

        bool wasAirborne = _movement && !_movement.IsGrounded();
        var rope = GetComponent<PlayerRope>();
        if (rope) rope.DetachFromRope();
        if (_movement) _movement.DetachFromMovingPlatform();
        if (_movement) _movement.Teleport(target);
        else
        {
            if (_rb) _rb.position = target;
            transform.position = target;
        }
        _inShadow = !_inShadow;
        _cooldown = swapCooldown;
        currentLane = (currentLane == Lane.OverWorld) ? Lane.ShadowWorld : Lane.OverWorld;

        OnLaneChanged?.Invoke(this, new OnLaneChangedEventArgs(currentLane));
        OnSwapAttempted?.Invoke(this, new SwapAttemptEventArgs(SwapOutcome.Succeeded, origin, target, wasAirborne));

        // Optional: tiny hitstop for juice
        // StartCoroutine(Hitstop(0.05f));
    }

    public void RestoreLane(Lane lane)
    {
        bool changed = currentLane != lane;
        currentLane = lane;
        _inShadow = lane == Lane.ShadowWorld;
        shadowTimer = shadowTimeLimit;
        _cooldown = 0f;
        UpdateGhost();
        UpdateCooldownUI();
        if (changed) OnLaneChanged?.Invoke(this, new OnLaneChangedEventArgs(lane));
    }

    void OnDrawGizmosSelected()
    {
        if (!playerCollider) return;
        Vector3 target = GetTargetPos();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(target, playerCollider.bounds.size * 0.98f);
    }
}
