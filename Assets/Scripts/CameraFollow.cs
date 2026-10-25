using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // Player reference
    public Vector3 offset = new Vector3(0, 0, -10f);
    public float smoothSpeed = 5f;

    private PlayerMovement2D player;
    private void OnEnable()
    {
        ShadowSwap.OnLaneChanged += OnLaneChanged;
        BindPlayer();
    }
    private void Start() => BindPlayer();
    private void BindPlayer()
    {
        if (player) player.OnPlayerRespawn -= OnRespawn;
        player = target ? target.GetComponent<PlayerMovement2D>() : null;
        if (player) player.OnPlayerRespawn += OnRespawn;
    }
    private void OnDisable()
    {
        ShadowSwap.OnLaneChanged -= OnLaneChanged;
        if (player) player.OnPlayerRespawn -= OnRespawn;
    }
    private void OnRespawn(object sender, System.EventArgs args)
    {
        if (target) transform.position = target.position + offset;
    }

    private void OnLaneChanged(object sender, ShadowSwap.OnLaneChangedEventArgs args)
    {
        if (sender is ShadowSwap swap && swap.transform == target)
            transform.position = target.position + offset;
    }

    void LateUpdate()
    {
        if (!target) return;
        Vector3 desired = target.position + offset;
        float blend = 1f - Mathf.Exp(-Mathf.Max(0f, smoothSpeed) * Time.deltaTime);
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, blend);
        transform.position = smoothed;
    }
}
