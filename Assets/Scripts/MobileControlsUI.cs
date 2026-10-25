using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>A customizable Canvas layout, also built automatically in phone gameplay scenes.</summary>
[RequireComponent(typeof(Canvas))]
public class MobileControlsUI : MonoBehaviour
{
    [Tooltip("Show phone buttons when testing Play Mode in the Unity Editor.")]
    public bool previewInEditor = true;
    [SerializeField] private RectTransform safeArea;
    [SerializeField] private CanvasGroup gameplayButtons;
    [SerializeField] private Text pauseLabel;
    [SerializeField] private GameObject ropeButtons;
    [SerializeField] private GameObject inventoryButtons;
    [SerializeField] private GameObject pauseButton;

    private Rect previousSafeArea;
    private Vector2Int previousScreenSize;
    private bool wasPaused;

    private void Start()
    {
        GetComponent<Canvas>().enabled = Application.isMobilePlatform || (Application.isEditor && previewInEditor);
        EnsureEventSystem();
        if (ropeButtons) ropeButtons.SetActive(FindFirstObjectByType<PlayerRope>() != null);
        if (inventoryButtons) inventoryButtons.SetActive(FindFirstObjectByType<PlayerInventory>() != null);
        if (pauseButton) pauseButton.SetActive(FindFirstObjectByType<PauseMenu>() != null);
        ApplySafeArea();
        UpdatePauseState();
    }

    private void Update()
    {
        if (Screen.safeArea != previousSafeArea || Screen.width != previousScreenSize.x || Screen.height != previousScreenSize.y)
            ApplySafeArea();
        if (wasPaused != (Time.timeScale <= 0f)) UpdatePauseState();
    }

    private void OnDisable() => MobileGameInput.Clear();

    private void ApplySafeArea()
    {
        if (!safeArea || Screen.width <= 0 || Screen.height <= 0) return;
        previousSafeArea = Screen.safeArea;
        previousScreenSize = new Vector2Int(Screen.width, Screen.height);
        Vector2 size = previousScreenSize;
        safeArea.anchorMin = previousSafeArea.min / size;
        safeArea.anchorMax = previousSafeArea.max / size;
        safeArea.offsetMin = Vector2.zero;
        safeArea.offsetMax = Vector2.zero;
    }

    private void UpdatePauseState()
    {
        wasPaused = Time.timeScale <= 0f;
        if (gameplayButtons)
        {
            gameplayButtons.interactable = !wasPaused;
            gameplayButtons.blocksRaycasts = !wasPaused;
            gameplayButtons.alpha = wasPaused ? 0.3f : 1f;
        }
        if (pauseLabel) pauseLabel.text = wasPaused ? "Resume" : "Pause";
    }

    public static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>()) return;
        var host = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        host.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
    }

    public static MobileControlsUI CreateDefault()
    {
        var root = new GameObject("Mobile Controls", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        // Above existing HUDs; only the individual button rectangles block touches.
        canvas.sortingOrder = 100;
        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;

        var ui = root.AddComponent<MobileControlsUI>();
        ui.safeArea = CreatePanel(root.transform, "Safe Area");
        var gameplay = CreatePanel(ui.safeArea, "Gameplay Buttons");
        ui.gameplayButtons = gameplay.gameObject.AddComponent<CanvasGroup>();

        CreateButton(gameplay, "Left", MobileGameInput.Control.MoveLeft, Vector2.zero, new Vector2(84, 84), new Vector2(104, 104));
        CreateButton(gameplay, "Right", MobileGameInput.Control.MoveRight, Vector2.zero, new Vector2(206, 84), new Vector2(104, 104));
        var bottomRight = new Vector2(1, 0);
        CreateButton(gameplay, "Jump", MobileGameInput.Control.Jump, bottomRight, new Vector2(-84, 84), new Vector2(104, 104));
        CreateButton(gameplay, "Swap", MobileGameInput.Control.Swap, bottomRight, new Vector2(-206, 84), new Vector2(104, 104));

        var rope = CreatePanel(gameplay, "Rope Buttons");
        ui.ropeButtons = rope.gameObject;
        CreateButton(rope, "Up", MobileGameInput.Control.ClimbUp, bottomRight, new Vector2(-325, 166), new Vector2(80, 80));
        CreateButton(rope, "Down", MobileGameInput.Control.ClimbDown, bottomRight, new Vector2(-325, 68), new Vector2(80, 80));
        CreateButton(rope, "Grab", MobileGameInput.Control.Interact, bottomRight, new Vector2(-437, 84), new Vector2(104, 104));

        var inventory = CreatePanel(gameplay, "Inventory Buttons");
        ui.inventoryButtons = inventory.gameObject;
        CreateButton(inventory, "Small\nBox", MobileGameInput.Control.SmallBox, bottomRight, new Vector2(-84, 222), new Vector2(104, 96));
        CreateButton(inventory, "Big\nBox", MobileGameInput.Control.BigBox, bottomRight, new Vector2(-206, 222), new Vector2(104, 96));

        var pause = CreateButton(ui.safeArea, "Pause", MobileGameInput.Control.Pause, Vector2.one, new Vector2(-82, -48), new Vector2(112, 64));
        ui.pauseButton = pause.gameObject;
        ui.pauseLabel = pause.GetComponentInChildren<Text>();
        return ui;
    }

    private static RectTransform CreatePanel(Transform parent, string name)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }

    private static RectTransform CreateButton(Transform parent, string label, MobileGameInput.Control control, Vector2 anchor, Vector2 position, Vector2 size)
    {
        var button = new GameObject(control.ToString(), typeof(RectTransform), typeof(Image));
        var rect = button.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        button.GetComponent<Image>().color = new Color(0.08f, 0.12f, 0.2f, 0.72f);
        button.AddComponent<MobileControlButton>().control = control;

        var textRect = CreatePanel(rect, "Label");
        var text = textRect.gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = label;
        text.fontSize = 25;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        return rect;
    }
}
