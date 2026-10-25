using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Readable swap feedback and shadow energy, installed in gameplay scenes.</summary>
[RequireComponent(typeof(Canvas))]
public class ShadowSenseUI : MonoBehaviour
{
    [SerializeField] private ShadowSwap swap;
    [SerializeField] private RectTransform safeArea;
    [SerializeField] private Text laneLabel;
    [SerializeField] private Text statusLabel;
    [SerializeField] private Text timerLabel;
    [SerializeField] private Text effectsLabel;
    [SerializeField] private Image energyBar;
    [SerializeField] private Image panel;
    [SerializeField] private Button effectsButton;

    private Rect previousSafeArea;
    private Vector2Int previousScreenSize;
    private float messageRemaining;
    private string message;
    private float pulseTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var player = FindFirstObjectByType<PlayerMovement2D>();
        if (!player || !player.TryGetComponent(out ShadowSwap ability)) return;
        if (!player.GetComponent<ShadowSwapEcho>()) player.gameObject.AddComponent<ShadowSwapEcho>();
        if (!FindFirstObjectByType<ShadowSenseUI>(FindObjectsInactive.Include)) Create(ability);
    }

    private void OnEnable()
    {
        if (swap) swap.OnSwapAttempted += OnSwapAttempted;
        if (effectsButton) effectsButton.onClick.AddListener(ToggleEffects);
    }

    private void OnDisable()
    {
        if (swap) swap.OnSwapAttempted -= OnSwapAttempted;
        if (effectsButton) effectsButton.onClick.RemoveListener(ToggleEffects);
    }

    private void OnSwapAttempted(object sender, ShadowSwap.SwapAttemptEventArgs args)
    {
        message = args.Outcome == ShadowSwap.SwapOutcome.Blocked ? "Destination blocked" :
                  args.Outcome == ShadowSwap.SwapOutcome.CoolingDown ? "Swap is recharging" :
                  swap.IsInShadow ? "Shadow entered" : "Returned to overworld";
        messageRemaining = 0.8f;
    }

    private void LateUpdate()
    {
        if (Screen.safeArea != previousSafeArea || Screen.width != previousScreenSize.x || Screen.height != previousScreenSize.y)
            ApplySafeArea();
        if (!swap) return;

        messageRemaining = Mathf.Max(0f, messageRemaining - Time.deltaTime);
        pulseTime += Time.deltaTime;
        bool shadow = swap.IsInShadow;
        float reserve = Mathf.Clamp01(swap.ShadowTimeRemaining / Mathf.Max(0.01f, swap.shadowTimeLimit));
        bool urgent = shadow && reserve <= 0.25f;
        SetText(laneLabel, shadow ? "SHADOW WORLD" : "OVERWORLD");
        SetText(statusLabel, messageRemaining > 0f ? message :
            swap.CooldownRemaining > 0f ? "Swap is recharging" :
            swap.DestinationClear ? "Swap ready" : "Destination blocked");
        int tenths = Mathf.CeilToInt(swap.ShadowTimeRemaining * 10f);
        // Rebuild timer text only when the visible tenth/label changes.
        if (tenths != lastTenths || shadow != lastShadow || urgent != lastUrgent)
        {
            lastTenths = tenths;
            lastShadow = shadow;
            lastUrgent = urgent;
            SetText(timerLabel, (urgent ? "Return now: " : shadow ? "Return in: " : "Shadow reserve: ") +
                (tenths / 10f).ToString("0.0") + "s");
        }
        SetText(effectsLabel, ShadowFeedbackSettings.ReducedMotion ? "FX Off" : "FX On");
        Color tint = urgent ? new Color(1f, 0.65f, 0.25f) :
            shadow ? new Color(0.75f, 0.55f, 1f) : new Color(0.35f, 0.9f, 1f);
        if (energyBar)
        {
            // A stretched solid Image needs no generated sprite or shader asset.
            energyBar.rectTransform.anchorMax = new Vector2(reserve, 1f);
            energyBar.color = tint;
        }
        if (panel)
        {
            float pulse = urgent && !ShadowFeedbackSettings.ReducedMotion ? 0.05f * (1f + Mathf.Sin(pulseTime * 6f)) : 0f;
            panel.color = new Color(0.04f + pulse, 0.055f, 0.1f, 0.86f);
        }
        if (timerLabel) timerLabel.color = tint;
    }

    private int lastTenths = -1;
    private bool lastShadow;
    private bool lastUrgent;

    private static void SetText(Text label, string value)
    {
        if (label && label.text != value) label.text = value;
    }

    private void ApplySafeArea()
    {
        if (!safeArea || Screen.width <= 0 || Screen.height <= 0) return;
        previousSafeArea = Screen.safeArea;
        previousScreenSize = new Vector2Int(Screen.width, Screen.height);
        Vector2 size = previousScreenSize;
        safeArea.anchorMin = previousSafeArea.min / size;
        safeArea.anchorMax = previousSafeArea.max / size;
        safeArea.offsetMin = safeArea.offsetMax = Vector2.zero;
    }

    public void ToggleEffects() => ShadowFeedbackSettings.ReducedMotion = !ShadowFeedbackSettings.ReducedMotion;

    public static ShadowSenseUI Create(ShadowSwap ability)
    {
        var root = new GameObject("Shadow Sense HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        // Configure serialized references before subscribing to the ability.
        root.SetActive(false);
        var ui = root.AddComponent<ShadowSenseUI>();
        ui.swap = ability;
        ui.safeArea = Rect(root.transform, "Safe Area", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        var card = Rect(ui.safeArea, "Shadow Sense", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -98), new Vector2(344, 124));
        ui.panel = card.gameObject.AddComponent<Image>();
        ui.panel.raycastTarget = false;
        ui.laneLabel = Label(card, "Lane", new Vector2(-34, 40), new Vector2(260, 28), 19);
        ui.statusLabel = Label(card, "Swap Status", new Vector2(0, 12), new Vector2(326, 26), 20);
        ui.timerLabel = Label(card, "Shadow Timer", new Vector2(0, -16), new Vector2(326, 26), 19);

        var track = Rect(card, "Energy Track", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -44), new Vector2(306, 8));
        track.gameObject.AddComponent<Image>().color = new Color(0.2f, 0.24f, 0.3f);
        track.GetComponent<Image>().raycastTarget = false;
        var fill = Rect(track, "Energy", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        ui.energyBar = fill.gameObject.AddComponent<Image>();
        ui.energyBar.raycastTarget = false;

        var toggle = Rect(card, "Effects Toggle", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(130, 40), new Vector2(70, 32));
        toggle.gameObject.AddComponent<Image>().color = new Color(0.18f, 0.23f, 0.32f);
        var button = toggle.gameObject.AddComponent<Button>();
        button.targetGraphic = toggle.GetComponent<Image>();
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        ui.effectsButton = button;
        ui.effectsLabel = Label(toggle, "Effects Label", Vector2.zero, new Vector2(70, 32), 16);
        root.SetActive(true);
        MobileControlsUI.EnsureEventSystem();
        return ui;
    }

    private static RectTransform Rect(Transform parent, string name, Vector2 min, Vector2 max, Vector2 position, Vector2 size)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }

    private static Text Label(Transform parent, string name, Vector2 position, Vector2 size, int fontSize)
    {
        var rect = Rect(parent, name, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, size);
        var label = rect.gameObject.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = fontSize;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.raycastTarget = false;
        return label;
    }
}
