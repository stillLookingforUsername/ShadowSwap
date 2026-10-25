using UnityEngine;
using UnityEngine.UI;

public class LaneObject : MonoBehaviour
{
    //public enum Lane { OverWorld, ShadowWorld }
    public ShadowSwap.Lane lane;

    private void OnEnable()
    {
        ShadowSwap.OnLaneChanged -= HandleLaneChanged;
        ShadowSwap.OnLaneChanged += HandleLaneChanged;
    }
    private void OnDestroy()
    {
        ShadowSwap.OnLaneChanged -= HandleLaneChanged;
    }

    private void HandleLaneChanged(object sender, ShadowSwap.OnLaneChangedEventArgs e)
    {
        if (sender is ShadowSwap swap && swap.gameObject.scene != gameObject.scene) return;
        gameObject.SetActive(lane == e.NewLane);
    }

    private void Start()
    {
        var player = PlayerMovement2D.Instance;
        if (player && player.TryGetComponent(out ShadowSwap swap)) gameObject.SetActive(lane == swap.currentLane);
    }

}
