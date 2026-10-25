using TMPro;
using UnityEngine;

public class OrbUIHint : MonoBehaviour
{
    public Transform player;
    public TextMeshProUGUI hintText;
    public float showDistance = 3f;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        if (!hintText) return;
        _canvasGroup = hintText.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = hintText.gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Update()
    {
        if (!player && PlayerMovement2D.Instance) player = PlayerMovement2D.Instance.transform;
        if (!player || !hintText || !_canvasGroup) return;
        float dist = Vector2.Distance(player.position, transform.position);
        float targetAlpha = dist < showDistance ? 1f : 0f;
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, targetAlpha, 1f - Mathf.Exp(-Time.deltaTime * 5f));

        //Billboard towards camera(if 3D camera in 2D game)
        hintText.transform.rotation = Quaternion.identity;
    }
}
