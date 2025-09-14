using UnityEngine;
using DG.Tweening;

public class DotweenUpDownMotion : MonoBehaviour
{
    [SerializeField] private float moveDistance = 3f;   //how far up/down
    [SerializeField] private float duration = 3f;   //how long it takes
    [SerializeField] private bool moveOnStart = true;

    private Tween platformTween;
    
    private void Start()
    {
        if (moveOnStart)
        {
            StartMoving();
        }
    }
    private void StartMoving()
    {
        //stop existing tween if any

        platformTween = transform.DOMoveY(transform.position.y + moveDistance, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopMoving()
    {
        platformTween?.Kill();
    }

}