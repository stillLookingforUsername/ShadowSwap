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
        _canvasGroup = hintText.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = hintText.gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Update()
    {
        float dist = Vector2.Distance(player.position, transform.position);
        float targetAlpha = dist < showDistance ? 1f : 0f;
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, targetAlpha, Time.deltaTime * 5f);

        //Billboard towards camera(if 3D camera in 2D game)
        hintText.transform.rotation = Quaternion.identity;
    }
}