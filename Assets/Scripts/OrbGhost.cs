using UnityEngine;

public class OrbGhost : MonoBehaviour
{
    public GameObject ghostSprite;
    public Transform player;
    public float triggerDistance = 3f;

    private void Update()
    {
        if (!player && PlayerMovement2D.Instance) player = PlayerMovement2D.Instance.transform;
        if (!player || !ghostSprite) return;
        bool show = Vector2.Distance(player.position, transform.position) < triggerDistance;
        ghostSprite.SetActive(show);
    }
}
