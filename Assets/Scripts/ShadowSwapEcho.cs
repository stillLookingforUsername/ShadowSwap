using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Two reusable sprite echoes, with no colliders or gameplay behavior.</summary>
[RequireComponent(typeof(ShadowSwap))]
public class ShadowSwapEcho : MonoBehaviour
{
    [Range(0.1f, 1f)] public float duration = 0.35f;
    [Range(0f, 0.7f)] public float opacity = 0.45f;
    public Color overworldColor = new Color(0.35f, 0.9f, 1f);
    public Color shadowColor = new Color(0.75f, 0.55f, 1f);

    private ShadowSwap swap;
    private SpriteRenderer source;
    private GameObject effectsRoot;
    private SpriteRenderer departure;
    private SpriteRenderer arrival;
    private Vector3 snapshotScale;
    private float elapsed;
    private bool playing;

    private void Awake()
    {
        swap = GetComponent<ShadowSwap>();
        source = GetComponent<SpriteRenderer>();
        if (!source) source = GetComponentInChildren<SpriteRenderer>();
        if (!source) return;

        effectsRoot = new GameObject("Shadow Swap Echoes");
        SceneManager.MoveGameObjectToScene(effectsRoot, gameObject.scene);
        departure = CreateEcho("Departure Echo");
        arrival = CreateEcho("Arrival Echo");
    }

    private SpriteRenderer CreateEcho(string name)
    {
        var echo = new GameObject(name).AddComponent<SpriteRenderer>();
        echo.transform.SetParent(effectsRoot.transform, false);
        echo.enabled = false;
        return echo;
    }

    private void OnEnable()
    {
        if (swap) swap.OnSwapAttempted += OnSwapAttempted;
    }

    private void OnDisable()
    {
        if (swap) swap.OnSwapAttempted -= OnSwapAttempted;
        HideEchoes();
    }

    private void OnDestroy()
    {
        if (effectsRoot) Destroy(effectsRoot);
    }

    private void OnSwapAttempted(object sender, ShadowSwap.SwapAttemptEventArgs args)
    {
        if (args.Outcome != ShadowSwap.SwapOutcome.Succeeded || !source || !source.sprite ||
            !source.enabled || ShadowFeedbackSettings.ReducedMotion) return;

        snapshotScale = source.transform.lossyScale;
        Vector3 offset = source.transform.position - transform.position;
        Color destinationColor = swap.IsInShadow ? shadowColor : overworldColor;
        Color originColor = swap.IsInShadow ? overworldColor : shadowColor;
        Snapshot(departure, args.Origin + offset, originColor);
        Snapshot(arrival, args.Destination + offset, destinationColor);
        elapsed = 0f;
        playing = true;
    }

    private void Snapshot(SpriteRenderer echo, Vector3 position, Color tint)
    {
        echo.sprite = source.sprite;
        echo.sharedMaterial = source.sharedMaterial;
        echo.flipX = source.flipX;
        echo.flipY = source.flipY;
        echo.sortingLayerID = source.sortingLayerID;
        echo.sortingOrder = source.sortingOrder + 1;
        echo.transform.SetPositionAndRotation(position, source.transform.rotation);
        echo.transform.localScale = snapshotScale;
        tint.a = opacity;
        echo.color = tint;
        echo.enabled = true;
    }

    private void Update()
    {
        if (!playing) return;
        if (ShadowFeedbackSettings.ReducedMotion)
        {
            HideEchoes();
            return;
        }
        elapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, duration));
        Fade(departure, progress);
        Fade(arrival, progress);
        if (progress >= 1f) HideEchoes();
    }

    private void Fade(SpriteRenderer echo, float progress)
    {
        Color tint = echo.color;
        tint.a = opacity * (1f - progress);
        echo.color = tint;
        echo.transform.localScale = snapshotScale * (1f + 0.15f * progress);
    }

    private void HideEchoes()
    {
        playing = false;
        if (departure) departure.enabled = false;
        if (arrival) arrival.enabled = false;
    }
}
