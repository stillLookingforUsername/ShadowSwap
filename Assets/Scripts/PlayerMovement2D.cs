using System;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private Rigidbody2D _rb;
    private float _move;
    private bool _jumpPressed;
    private float _coyoteTimer;
    private float _bufferTimer;
    private Vector3 _respawnPoint;

    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        _respawnPoint = transform.position;
    }

    private void Update()
    {
        bool grounded = IsGrounded();
        _coyoteTimer = grounded ? coyoteTime : Mathf.Max(0, _coyoteTimer - Time.deltaTime);
        _bufferTimer = _jumpPressed ? jumpBuffer : Mathf.Max(0, _bufferTimer - Time.deltaTime);

        if (_bufferTimer > 0 && _coyoteTimer > 0)
        {
            Jump();
            _bufferTimer = 0;
            _coyoteTimer = 0;
        }
    }

    private void FixedUpdate()
    {
        float target = _move * moveSpeed;
        float control = IsGrounded() ? 1f : airControlMultiplier;
        _rb.linearVelocity = new Vector2(Mathf.Lerp(_rb.linearVelocity.x, target, 0.2f * control), _rb.linearVelocity.y);
    }

    public bool IsGrounded()
    {
        Vector2 pos = groundCheck ? (Vector2)groundCheck.position : (Vector2)transform.position + Vector2.down * 0.6f;
        return Physics2D.OverlapBox(pos, groundCheckSize, 0f, groundMask);
    }

    private void Jump()
    {
        Vector2 v = _rb.linearVelocity;
        v.y = jumpForce;
        _rb.linearVelocity = v;
    }

    // Input System callbacks
    //public void OnMove(InputValue v) => _move = v.isPressed ? v.Get<float>() : 0f;
    //public void OnJump(InputValue v) { if (v.isPressed) _jumpPressed = true; else _jumpPressed = false; if (v.isPressed) _bufferTimer = jumpBuffer; }
    public void OnMove(InputValue v)
    {
        _move = v.Get<float>();
    }
    public void OnJump(InputValue v)
    {
        if (v.isPressed)
        {
            _bufferTimer = jumpBuffer;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Collectible collectible))
        {
            OnCoinPickUp?.Invoke(this, EventArgs.Empty);
            collectible.DestroySelf();
        }
    }

    public void SetRespawn(Vector3 newPoint)
    {
        _respawnPoint = newPoint;
    }
    public float GetMoveInput()
    {
        return _move;
    }
    public Vector2 GetLinearVelocity()
    {
        return _rb.linearVelocity;
    }
    public void Respawn()
    {
        transform.position = _respawnPoint;
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


