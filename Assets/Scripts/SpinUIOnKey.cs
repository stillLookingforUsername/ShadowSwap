using DG.Tweening;
using UnityEngine;

public class SpinUIOnKey : MonoBehaviour
{
    [SerializeField] private RectTransform targetUI; 
    [SerializeField] private float spinAngle = 45f;     //how far it spin
    [SerializeField] private float spinDuration = 0.3f;     //how fast it spin

    private Tween spinTween;

    private void OnEnable()
    {
        ShadowSwap.OnLaneChanged += HandleSpin;
    }
    private void OnDisable()
    {
        ShadowSwap.OnLaneChanged -= HandleSpin;
    }

    private void HandleSpin(object sender,ShadowSwap.OnLaneChangedEventArgs e)
    {
        spinTween?.Kill();  //kill any previous tween if already running

        //spin clockwise then back to orignal
        spinTween = targetUI.DORotate(new Vector3(0, 0, spinAngle), spinDuration)
                            .SetEase(Ease.OutQuad)
                            .SetLoops(2, LoopType.Yoyo);    //spin & come back

    }
}