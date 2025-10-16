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

    private void Start()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * moveDistance;

        //up & down loop
        _moveTween = transform.DOMoveY(endPos.y, moveDuration)
            .SetEase(moveEase)
            .SetLoops(-1, LoopType.Yoyo);
        orbEvent.OnOrbDetectedPlayer += OnDestroy;
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
            GetComponent<SpriteRenderer>().DOFade(1f, 0.5f); //show
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.TryGetComponent<PlayerMovement2D>(out PlayerMovement2D player))
        {
            GetComponent<SpriteRenderer>().DOFade(0f, 0.5f);    //hide
        }
    }


    private void OnDestroy()
    {
        if(_moveTween != null && _moveTween.IsActive())
        {
            _moveTween.Kill();
        }
        Destroy(this.gameObject);
    }
}
