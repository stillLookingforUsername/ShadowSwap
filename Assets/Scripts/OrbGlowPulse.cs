using UnityEngine;
using DG.Tweening;

public class OrbGlowPulse : MonoBehaviour
{
    private Tween pulseTween;
    private void Start()
    {
        // Loop pulsing effect
        pulseTween = transform.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void OnDestroy() => pulseTween?.Kill();
}
