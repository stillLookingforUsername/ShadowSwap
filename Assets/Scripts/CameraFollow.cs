using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // Player reference
    public Vector3 offset = new Vector3(0, 0, -10f);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (!target) return;
        Vector3 desired = target.position + offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
        transform.position = smoothed;
    }
}
