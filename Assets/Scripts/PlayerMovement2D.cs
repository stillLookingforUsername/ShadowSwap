using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovement2D : MonoBehaviour
{
    public static PlayerMovement2D Instance { get; private set; }
    [Header("Move")]
    public float moveSpeed = 8f;
    public float airControlMultiplier = 0.7f;
    [Header("Jump")]
    public float jumpForce = 14f;
    public float coyoteTime = 0.1f;
    public float jumpBuffer = 0.1f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.6f, 0.1f);
    public event EventHandler OnCoinPickUp;
    public event EventHandler OnPlayerJump;
    public event Action<Collectible> OnCollectiblePickedUp;
    public event EventHandler OnPlayerRespawn;

    private Rigidbody2D _rb;
    private float _move;
    private float _coyoteTimer;
    private float _bufferTimer;
    private bool _jumpConsumedGround;
    private Vector3 _respawnPoint;
    private ShadowSwap.Lane _respawnLane;
    private float _normalGravity;
    private float _normalScaleY;
    private Transform _spawnParent;
    private Quaternion _spawnRotation;
    private PlayerRope _rope;
    private readonly HashSet<UnityEngine.Object> _controlLocks = new HashSet<UnityEngine.Object>();
    public OrbTransport Carrier { get; private set; }
    public bool IsRopeAttached => _rope && _rope.IsAttached;
    public bool CanAcceptInput => isActiveAndEnabled && _rb && _rb.simulated && !Carrier && _controlLocks.Count == 0 &&
        !SceneNavigation.IsLoading && !LevelRunTracker.HasOpenResults;
    private float GravityDirection => _rb.gravityScale < 0f ? -1f : 1f;
	
	private Animator _animator;
	private bool _isGrounded;

    private PlayerAudio playerAudio;

    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        playerAudio = GetComponent<PlayerAudio>();
		_animator = GetComponent<Animator>();
        _rope = GetComponent<PlayerRope>();
        _normalGravity = _rb.gravityScale;
        _normalScaleY = transform.localScale.y;
        _spawnParent = transform.parent;
        _spawnRotation = transform.rotation;
        _respawnPoint = transform.position;
    }
    private void Start()
    {
        // Spawn state is captured in Awake, before a trigger can set a checkpoint.
    }

    private void OnEnable()
    {
        MobileGameInput.ActionPressed += OnTouchAction;
        MobileGameInput.ResetRequested += ClearInput;
    }

    private void OnDisable()
    {
        MobileGameInput.ActionPressed -= OnTouchAction;
        MobileGameInput.ResetRequested -= ClearInput;
        ClearInput();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        bool grounded = IsGrounded();

        if (CanAcceptInput) UpdateAnimationParameters();

        //walking sound
        bool walking = CanAcceptInput && !IsRopeAttached && Time.timeScale > 0f && Mathf.Abs(_rb.linearVelocity.x) > 0.1f && grounded;
        if (playerAudio != null)
        {
            playerAudio.HandleWalkSound(walking);
        }
    }

    private void FixedUpdate()
    {
        if (Time.timeScale <= 0f) return;
        if (!CanAcceptInput || IsRopeAttached)
        {
            _coyoteTimer = 0f;
            _bufferTimer = Mathf.Max(0f, _bufferTimer - Time.fixedDeltaTime);
            return;
        }
        bool grounded = IsGrounded();
        if (!grounded) _jumpConsumedGround = false;
        _coyoteTimer = grounded && !_jumpConsumedGround ? coyoteTime : Mathf.Max(0f, _coyoteTimer - Time.fixedDeltaTime);
        float target = GetMoveInput() * moveSpeed;
        float control = grounded ? 1f : airControlMultiplier;
        // Preserve the existing response at the project's 0.02 s physics step.
        float blend = 1f - Mathf.Pow(1f - Mathf.Clamp01(0.2f * control), Time.fixedDeltaTime / 0.02f);
        _rb.linearVelocity = new Vector2(Mathf.Lerp(_rb.linearVelocity.x, target, blend), _rb.linearVelocity.y);

        if (_bufferTimer > 0 && _coyoteTimer > 0)
        {
            Jump();
            _bufferTimer = 0;
            _coyoteTimer = 0;
            _jumpConsumedGround = true;
        }
        _bufferTimer = Mathf.Max(0f, _bufferTimer - Time.fixedDeltaTime);
    }

    public bool IsGrounded()
    {
        Vector2 bodyPosition = _rb ? _rb.position : (Vector2)transform.position;
        Vector2 pos = groundCheck ? (Vector2)groundCheck.position + bodyPosition - (Vector2)transform.position :
            bodyPosition + Vector2.down * 0.6f * (_rb ? GravityDirection : 1f);
        return Physics2D.OverlapBox(pos, groundCheckSize, 0f, groundMask);
    }

    private void Jump()
    {
        Vector2 v = _rb.linearVelocity;
        v.y = jumpForce * GravityDirection;
        _rb.linearVelocity = v;
        OnPlayerJump?.Invoke(this, EventArgs.Empty);

        //Play jump sound
        if (playerAudio != null)
        {
            playerAudio.PlayJump();
        }

        if (_animator != null)
        {
            _animator.SetTrigger("JumpAnimParam");
        }
    }

    private void UpdateAnimationParameters()
    {
        if (_animator == null) return;

        _isGrounded = IsGrounded();
        float speed = Mathf.Abs(_rb.linearVelocity.x);

        _animator.SetFloat("SpeedAnimParam", speed);
        _animator.SetBool("IsGroundedAnimParam", _isGrounded);

        float move = GetMoveInput();
        if (Mathf.Abs(move) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(move) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

    }

    // Keyboard callbacks and touch buttons use the same movement/jump path.
    public void OnMove(InputValue v)
    {
        _move = CanAcceptInput && Time.timeScale > 0f ? v.Get<float>() : 0f;
    }
    public void OnJump(InputValue v)
    {
        if (v.isPressed)
        {
            RequestJump();
        }
    }

    public void RequestJump()
    {
        if (!CanAcceptInput || Time.timeScale <= 0f) return;
        if (_rope) _rope.DetachFromRope();
        DetachFromMovingPlatform();
        _bufferTimer = jumpBuffer;
    }

    private void OnTouchAction(MobileGameInput.Control control)
    {
        if (control == MobileGameInput.Control.Jump) RequestJump();
    }

    private void ClearInput()
    {
        _move = 0f;
        _bufferTimer = 0f;
        _coyoteTimer = 0f;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.timeScale > 0f && !LevelRunTracker.HasOpenResults &&
            other.gameObject.TryGetComponent(out Collectible collectible) && collectible.TryCollect())
        {
            OnCoinPickUp?.Invoke(this, EventArgs.Empty);
            OnCollectiblePickedUp?.Invoke(collectible);
            collectible.DestroySelf();
        }
    }

    public void SetRespawn(Vector3 newPoint)
    {
        _respawnPoint = newPoint;
        var shadow = GetComponent<ShadowSwap>();
        _respawnLane = shadow ? shadow.currentLane : ShadowSwap.Lane.OverWorld;
    }
    public float GetMoveInput()
    {
        return CanAcceptInput && Time.timeScale > 0f ? MobileGameInput.ResolveHorizontal(_move) : 0f;
    }
    public Vector2 GetLinearVelocity()
    {
        return _rb.linearVelocity;
    }
    public void Respawn()
    {
        if (LevelRunTracker.HasOpenResults) return;
        if (Carrier) Carrier.ReleasePlayer();
        if (_rope) _rope.DetachFromRope();
        var gravity = GetComponent<GravityFlipState>();
        if (gravity) gravity.Restore();
        _controlLocks.Clear();
        MobileGameInput.Clear();
        ClearInput();
        _jumpConsumedGround = false;
        transform.SetParent(_spawnParent ? _spawnParent : null, true);
        transform.rotation = _spawnRotation;
        Vector3 scale = transform.localScale;
        scale.y = _normalScaleY;
        transform.localScale = scale;
        _rb.simulated = true;
        _rb.gravityScale = _normalGravity;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        Teleport(_respawnPoint);
        var shadow = GetComponent<ShadowSwap>();
        if (shadow) shadow.RestoreLane(_respawnLane);
        OnPlayerRespawn?.Invoke(this, EventArgs.Empty);
    }

    public void SetCarrier(OrbTransport carrier)
    {
        Carrier = carrier;
        ClearInput();
    }

    public void DetachFromMovingPlatform()
    {
        if (transform.parent && transform.parent.GetComponent<MovingPlatform>()) transform.SetParent(null, true);
    }

    public void AcquireControlLock(UnityEngine.Object owner)
    {
        if (owner) _controlLocks.Add(owner);
        ClearInput();
    }

    public void ReleaseControlLock(UnityEngine.Object owner) => _controlLocks.Remove(owner);

    public void Teleport(Vector3 position)
    {
        // Teleport the physics body too, so interpolation cannot trail across lanes.
        _rb.position = position;
        transform.position = position;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}


