using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovement2D : MonoBehaviour
{
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

    Rigidbody2D _rb;
    float _move;
    bool _jumpPressed;
    float _coyoteTimer;
    float _bufferTimer;

    void Awake() { _rb = GetComponent<Rigidbody2D>(); }
    void Update()
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

    void FixedUpdate()
    {
        float target = _move * moveSpeed;
        float control = IsGrounded() ? 1f : airControlMultiplier;
        _rb.linearVelocity = new Vector2(Mathf.Lerp(_rb.linearVelocity.x, target, 0.2f * control), _rb.linearVelocity.y);
    }

    bool IsGrounded()
    {
        Vector2 pos = groundCheck ? (Vector2)groundCheck.position : (Vector2)transform.position + Vector2.down * 0.6f;
        return Physics2D.OverlapBox(pos, groundCheckSize, 0f, groundMask);
    }

    void Jump()
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

    void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}


