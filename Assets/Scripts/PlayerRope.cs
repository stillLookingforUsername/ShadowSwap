using UnityEngine;

public class PlayerRope : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private HingeJoint2D ropeJoint;
    private Rigidbody2D rb;
    private bool isAttached;

    public float swingForce = 5f;
    public float climbSpeed = 3f;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isAttached)
        {
            //swing control
            float move = inputActions.Player.Move.ReadValue<float>();
            rb.AddForce(new Vector2(move * swingForce, 0f));

            //climb control
            float climb = inputActions.Player.Vertical.ReadValue<float>();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, climb * climbSpeed);


            //Detach from rope
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DetachFromRope();
            }
        }
    }

     private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("RopeEnd") && !isAttached)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                AttachToRope(collision.attachedRigidbody);
            }
        }
    }

    private void AttachToRope(Rigidbody2D ropeSegment)
    {
        ropeJoint = gameObject.AddComponent<HingeJoint2D>();
        ropeJoint.connectedBody = ropeSegment;
        ropeJoint.autoConfigureConnectedAnchor = false;
        ropeJoint.anchor = Vector2.zero;
        ropeJoint.connectedAnchor = Vector2.zero;
        isAttached = true;
    }

    private void DetachFromRope()
    {
        if (ropeJoint)
        {
            Destroy(ropeJoint);
            isAttached = false;
        }
    }
}