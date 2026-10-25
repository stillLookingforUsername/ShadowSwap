using UnityEngine;

/// <summary>Identifies a rope end without requiring project tags.</summary>
public class RopeGrabPoint : MonoBehaviour
{
    public Rigidbody2D Body => GetComponentInParent<Rigidbody2D>();
}
