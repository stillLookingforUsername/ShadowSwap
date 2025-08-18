/*using UnityEditor.Callbacks;
using UnityEngine;

public class EnemyOctopus : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed;
    public float jumpSpeed;
    public LayerMask whatIsGround;

    private Rigidbody2D _rb;
    private bool _isGrounded;
    private bool _shouldJump;
    private float jumpForce = 3f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //is grounded
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, whatIsGround);

        //player direction
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        //player above detection
        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 << player.gameObject.layer);

        if (_isGrounded)
        {
            //chase player
            _rb.linearVelocity = new Vector2(direction * chaseSpeed, _rb.linearVelocity.y);

            //jump if there is a gap ahead && no ground infront
            //else if there is a player above and platform above

            //if grounded
            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, whatIsGround);

            //if gap
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, whatIsGround);

            //if platform above
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 10f, whatIsGround);

            if (!groundInFront.collider && !gapAhead.collider)
            {
                _shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                _shouldJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (_isGrounded && _shouldJump)
        {
            _shouldJump = false;
            Vector2 direction = (player.position - transform.position).normalized;

            Vector2 jumpDirection = direction * jumpForce;

            _rb.AddForce(new Vector2(jumpDirection.x, jumpForce),ForceMode2D.Impulse);
        }
    }
}
*/


using UnityEngine;

public class EnemyOctopus : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed = 2f;
    public float jumpForce = 5f;
    public LayerMask whatIsGround;

    private Rigidbody2D _rb;
    private bool _isGrounded;
    private bool _shouldJump;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Ground check (raycast down)
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, whatIsGround);

        // Player direction
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        if (_isGrounded)
        {
            // Chase player horizontally
            _rb.linearVelocity = new Vector2(direction * chaseSpeed, _rb.linearVelocity.y);

            // --- Gap / ground detection ---
            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, whatIsGround);
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, whatIsGround);

            // --- Player above detection ---
            bool isPlayerAbove = player.position.y > transform.position.y + 1f;

            // Decide when to jump
            if (!groundInFront.collider && !gapAhead.collider)
            {
                // Jump across gap
                _shouldJump = true;
            }
            else if (isPlayerAbove)
            {
                // Jump because player is above
                _shouldJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (_isGrounded && _shouldJump)
        {
            _shouldJump = false;

            // Jump straight up (if you want diagonal chase jump, we can adjust this later)
            _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Debug rays in Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1f); // ground check
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 3f);   // player above zone
    }
}
