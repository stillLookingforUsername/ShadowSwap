using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GravityFlipState : MonoBehaviour
{
    private Rigidbody2D body;
    private float originalGravity;
    private float originalScaleY;
    private float remaining;
    public bool IsFlipped { get; private set; }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        originalGravity = body.gravityScale;
        originalScaleY = transform.localScale.y;
    }

    public void Flip(float duration)
    {
        if (!IsFlipped)
        {
            originalGravity = body.gravityScale;
            originalScaleY = transform.localScale.y;
            body.gravityScale = -originalGravity;
            SetScaleY(-originalScaleY);
            IsFlipped = true;
        }
        // Re-entering or overlapping traps refreshes one effect instead of stacking inversions.
        remaining = Mathf.Max(0f, duration);
        if (remaining == 0f) Restore();
    }

    private void Update()
    {
        if (!IsFlipped) return;
        remaining -= Time.deltaTime;
        if (remaining <= 0f) Restore();
    }

    public void Restore()
    {
        if (!IsFlipped || !body) return;
        body.gravityScale = originalGravity;
        SetScaleY(originalScaleY);
        IsFlipped = false;
        remaining = 0f;
    }

    private void SetScaleY(float y)
    {
        Vector3 scale = transform.localScale;
        scale.y = y;
        transform.localScale = scale;
    }

    private void OnDisable() => Restore();
}
