using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRope : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private HingeJoint2D ropeJoint;
    private Rigidbody2D rb;
    private bool isAttached;
    private readonly HashSet<Collider2D> nearbyRopes = new HashSet<Collider2D>();

    public float swingForce = 5f;
    public float climbSpeed = 3f;
    private PlayerMovement2D movement;
    public bool IsAttached => isAttached && ropeJoint && ropeJoint.connectedBody;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement2D>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        MobileGameInput.ActionPressed += OnTouchAction;
    }
    private void OnDisable()
    {
        inputActions.Disable();
        MobileGameInput.ActionPressed -= OnTouchAction;
        nearbyRopes.Clear();
        DetachFromRope();
    }

    private void OnDestroy() => inputActions?.Dispose();

    private void Update()
    {
        if (Time.timeScale <= 0f || Keyboard.current == null) return;
        if (isAttached && Keyboard.current.spaceKey.wasPressedThisFrame) DetachFromRope();
        if (!isAttached && Keyboard.current.fKey.wasPressedThisFrame) RequestGrab();
    }

    private void FixedUpdate()
    {
        if (isAttached && !IsAttached) DetachFromRope();
        if (!IsAttached || Time.timeScale <= 0f || !rb.simulated || (movement && !movement.CanAcceptInput)) return;
        // Apply rope forces at the physics rate, independently of display FPS.
        float move = MobileGameInput.ResolveHorizontal(inputActions.Player.Move.ReadValue<float>());
        rb.AddForce(new Vector2(move * swingForce, 0f));
        float climb = MobileGameInput.ResolveVertical(inputActions.Player.Vertical.ReadValue<float>());
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, climb * climbSpeed);
    }

    private void OnTouchAction(MobileGameInput.Control control)
    {
        if (control == MobileGameInput.Control.Interact) RequestGrab();
        if (control == MobileGameInput.Control.Jump && isAttached) DetachFromRope();
    }

    private void OnTriggerEnter2D(Collider2D collision) => TrackRope(collision);
    private void OnTriggerStay2D(Collider2D collision) => TrackRope(collision);
    private void OnTriggerExit2D(Collider2D collision) => nearbyRopes.Remove(collision);

    private void TrackRope(Collider2D collision)
    {
        // String comparison also tolerates scenes where the optional rope tag is absent.
        if (collision.GetComponentInParent<RopeGrabPoint>() || collision.tag == "RopeEnd" || collision.tag == "RopeGrab")
            nearbyRopes.Add(collision);
    }

    public void RequestGrab()
    {
        if (!isActiveAndEnabled || Time.timeScale <= 0f || IsAttached || (movement && !movement.CanAcceptInput)) return;
        foreach (Collider2D rope in nearbyRopes)
        {
            if (!rope || !rope.enabled || !rope.gameObject.activeInHierarchy) continue;
            var marker = rope.GetComponentInParent<RopeGrabPoint>();
            Rigidbody2D segment = marker ? marker.Body : rope.attachedRigidbody;
            if (!segment) continue;
            AttachToRope(segment);
            return;
        }
    }

    private void AttachToRope(Rigidbody2D ropeSegment)
    {
        if (movement) movement.DetachFromMovingPlatform();
        ropeJoint = gameObject.AddComponent<HingeJoint2D>();
        ropeJoint.connectedBody = ropeSegment;
        ropeJoint.autoConfigureConnectedAnchor = false;
        ropeJoint.anchor = Vector2.zero;
        ropeJoint.connectedAnchor = Vector2.zero;
        isAttached = true;
    }

    public void DetachFromRope()
    {
        if (ropeJoint)
        {
            ropeJoint.enabled = false;
            Destroy(ropeJoint);
        }
        ropeJoint = null;
        isAttached = false;
    }
}
