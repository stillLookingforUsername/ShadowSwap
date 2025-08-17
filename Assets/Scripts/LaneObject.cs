using UnityEngine;
using UnityEngine.UI;

public class LaneObject : MonoBehaviour
{
    //public enum Lane { OverWorld, ShadowWorld }
    public ShadowSwap.Lane lane;

    private void OnEnable()
    {
        ShadowSwap.OnLaneChanged += HandleLaneChanged;
    }
    private void OnDisable()
    {
        ShadowSwap.OnLaneChanged -= HandleLaneChanged;
    }

    private void HandleLaneChanged(object sender, ShadowSwap.OnLaneChangedEventArgs e)
    {
        gameObject.SetActive(lane == e.NewLane);
    }

}
