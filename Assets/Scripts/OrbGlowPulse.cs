using UnityEngine;
using DG.Tweening;

public class OrbGlowPulse : MonoBehaviour
{
    private void Start()
    {
        // Loop pulsing effect
        transform.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}