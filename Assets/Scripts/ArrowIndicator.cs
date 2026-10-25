using UnityEngine;
using DG.Tweening;

public class ArrowIndicator : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 0.5f;
    public float moveDuration = 1f;
    public OrbEvent orbEvent;

    public Ease moveEase = Ease.InOutSine;

    private Tween _moveTween;
    private Tween _fadeTween;
    private SpriteRenderer spriteRenderer;
    private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();
    private void OnEnable() { if (orbEvent) orbEvent.OnOrbDetectedPlayer += CompleteHint; }
    private void OnDisable()
    {
        if (orbEvent) orbEvent.OnOrbDetectedPlayer -= CompleteHint;
        _fadeTween?.Kill();
    }
    private void CompleteHint() { if (this) Destroy(gameObject); }

    private void Start()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * moveDistance;

        //up & down loop
        _moveTween = transform.DOMoveY(endPos.y, moveDuration)
            .SetEase(moveEase)
            .SetLoops(-1, LoopType.Yoyo);
        /*
        transform.DOScale(1.1f, moveDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
            */
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerMovement2D>(out PlayerMovement2D player))
        {
            FadeTo(1f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.TryGetComponent<PlayerMovement2D>(out PlayerMovement2D player))
        {
            FadeTo(0f);
        }
    }


    private void OnDestroy()
    {
        if(_moveTween != null && _moveTween.IsActive())
        {
            _moveTween.Kill();
        }
        _fadeTween?.Kill();
    }

    private void FadeTo(float alpha)
    {
        if (!spriteRenderer) return;
        _fadeTween?.Kill();
        _fadeTween = spriteRenderer.DOFade(alpha, 0.5f);
    }
}
