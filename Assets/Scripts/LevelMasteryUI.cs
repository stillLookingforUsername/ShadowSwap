using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class LevelMasteryUI : MonoBehaviour
{
    [SerializeField] private LevelRunTracker tracker;
    [SerializeField] private RectTransform safeArea;
    [SerializeField] private GameObject challengeCard;
    [SerializeField] private Text runLabel;
    [SerializeField] private Text progressLabel;
    [SerializeField] private GameObject resultsBackdrop;
    [SerializeField] private RectTransform resultsCard;
    [SerializeField] private Text levelLabel;
    [SerializeField] private Text summaryLabel;
    [SerializeField] private Text badgesLabel;
    [SerializeField] private Text recordLabel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button menuButton;

    private Rect previousSafeArea;
    private Vector2Int previousScreenSize;
    private float refreshRemaining;
    private bool showingResults;

    private void OnEnable()
    {
        if (continueButton) continueButton.onClick.AddListener(OnContinue);
        if (replayButton) replayButton.onClick.AddListener(OnReplay);
        if (menuButton) menuButton.onClick.AddListener(OnMenu);
    }

    private void OnDisable()
    {
        if (continueButton) continueButton.onClick.RemoveListener(OnContinue);
        if (replayButton) replayButton.onClick.RemoveListener(OnReplay);
        if (menuButton) menuButton.onClick.RemoveListener(OnMenu);
    }

    private void Update()
    {
        if (previousSafeArea != Screen.safeArea || previousScreenSize.x != Screen.width || previousScreenSize.y != Screen.height)
            ApplySafeArea();
        if (showingResults) return;
        refreshRemaining -= Time.unscaledDeltaTime;
        if (refreshRemaining <= 0f)
        {
            RefreshHUD();
            refreshRemaining = 0.1f;
        }
    }

    private void RefreshHUD()
    {
        if (!tracker) tracker = LevelRunTracker.Instance;
        if (!tracker || tracker.Attempt == null) return;
        LevelAttempt attempt = tracker.Attempt;
        if (runLabel) runLabel.text = "ECHO MASTERY  " + FormatTime(attempt.ElapsedSeconds);
        if (progressLabel)
            progressLabel.text = (attempt.TotalPickups > 0 ? attempt.Pickups + "/" + attempt.TotalPickups + " cookies | " : "") +
                attempt.AirSwaps + "/" + attempt.AirSwapGoal + " air swaps | " + (attempt.Deaths == 0 ? "Clean run" : "Deaths: " + attempt.Deaths);
    }

    public void ShowResults(LevelMasteryCompletion completion)
    {
        if (!tracker) tracker = LevelRunTracker.Instance;
        if (!tracker) return;
        showingResults = true;
        if (challengeCard) challengeCard.SetActive(false);
        if (resultsBackdrop) resultsBackdrop.SetActive(true);
        if (resultsCard) resultsCard.gameObject.SetActive(true);
        LevelRunSummary run = completion.Summary;
        if (levelLabel) levelLabel.text = tracker.gameObject.scene.name;
        if (summaryLabel)
            summaryLabel.text = "Time: " + FormatTime(run.ElapsedSeconds) + "   |   Deaths: " + run.Deaths +
                "\nCookies: " + run.Pickups + "/" + run.TotalPickups + "   |   Swaps: " + run.Swaps + " (" + run.AirSwaps + " midair)";
        if (badgesLabel)
            badgesLabel.text = BadgeLine("Clean Run", LevelMasteryBadge.CleanRun, completion, "finish without dying") + "\n" +
                BadgeLine("Collector", LevelMasteryBadge.Collector, completion,
                    run.TotalPickups > 0 ? run.Pickups + "/" + run.TotalPickups + " cookies" : "no cookies in this room") + "\n" +
                BadgeLine("Shadow Dancer", LevelMasteryBadge.ShadowDancer, completion, run.AirSwaps + "/" + run.AirSwapGoal + " midair swaps");
        if (recordLabel)
            recordLabel.text = (completion.FirstClear ? "First clear saved!" : completion.PersonalBest ? "New personal best!" : "Keep improving!") +
                "  Best: " + FormatTime(completion.Record.bestSeconds) + "\nSaved badges: " +
                CountBadges(completion.Record.badges & run.AvailableBadges) + "/" + CountBadges(run.AvailableBadges) +
                "   |   Completed: " + completion.Record.completions;
        SetNavigationEnabled(true);
        var selected = continueButton && continueButton.interactable ? continueButton : replayButton;
        if (EventSystem.current && selected && selected.interactable) EventSystem.current.SetSelectedGameObject(selected.gameObject);
        ApplySafeArea();
    }

    private static string BadgeLine(string name, LevelMasteryBadge badge, LevelMasteryCompletion completion, string unmet)
    {
        if ((completion.Summary.EarnedBadges & badge) != 0)
            return "<color=#74EDDA>" + ((completion.NewBadges & badge) != 0 ? "NEW! " : "Earned: ") + name + "</color>";
        return name + " - " + unmet;
    }

    public static int CountBadges(LevelMasteryBadge badges)
    {
        int value = (int)badges;
        int count = 0;
        while (value != 0) { value &= value - 1; count++; }
        return count;
    }

    public static string FormatTime(float seconds)
    {
        if (float.IsNaN(seconds) || float.IsInfinity(seconds)) return "--:--.-";
        int tenths = Mathf.FloorToInt(Mathf.Max(0f, seconds) * 10f);
        return (tenths / 600) + ":" + ((tenths % 600) / 10).ToString("00") + "." + (tenths % 10);
    }

    private void OnContinue() { if (tracker && tracker.Continue()) SetNavigationEnabled(false); }
    private void OnReplay() { if (tracker && tracker.Replay()) SetNavigationEnabled(false); }
    private void OnMenu() { if (tracker && tracker.MainMenu()) SetNavigationEnabled(false); }

    private void SetNavigationEnabled(bool enabled)
    {
        if (continueButton) continueButton.interactable = enabled && ValidScene(tracker.NextSceneIndex);
        if (replayButton) replayButton.interactable = enabled && ValidScene(tracker.gameObject.scene.buildIndex);
        if (menuButton) menuButton.interactable = enabled && ValidScene(LevelRunTracker.MainMenuIndex());
    }

    private static bool ValidScene(int index) => index >= 0 && index < SceneManager.sceneCountInBuildSettings;

    private void ApplySafeArea()
    {
        if (!safeArea || Screen.width <= 0 || Screen.height <= 0) return;
        previousSafeArea = Screen.safeArea;
        previousScreenSize = new Vector2Int(Screen.width, Screen.height);
        Vector2 screenSize = previousScreenSize;
        safeArea.anchorMin = previousSafeArea.min / screenSize;
        safeArea.anchorMax = previousSafeArea.max / screenSize;
        safeArea.offsetMin = safeArea.offsetMax = Vector2.zero;
        if (resultsCard)
        {
            Canvas.ForceUpdateCanvases();
            resultsCard.sizeDelta = new Vector2(650f, 480f);
            float scale = Mathf.Min(1f, Mathf.Max(1f, safeArea.rect.width - 24f) / 650f,
                Mathf.Max(1f, safeArea.rect.height - 24f) / 480f);
            resultsCard.localScale = Vector3.one * scale;
        }
    }

    public static LevelMasteryUI Create(LevelRunTracker runTracker)
    {
        var root = new GameObject("Echo Mastery UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;
        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        root.SetActive(false);
        var ui = root.AddComponent<LevelMasteryUI>();
        ui.tracker = runTracker;
        ui.safeArea = Stretch(root.transform, "Safe Area");

        var challenge = Rect(ui.safeArea, "Challenges", new Vector2(0.5f, 1), new Vector2(0, -194), new Vector2(344, 54));
        var background = challenge.gameObject.AddComponent<Image>();
        background.color = new Color(0.04f, 0.055f, 0.1f, 0.76f);
        background.raycastTarget = false;
        ui.challengeCard = challenge.gameObject;
        ui.runLabel = Label(challenge, "Attempt Time", new Vector2(0, 11), new Vector2(330, 24), 18);
        ui.runLabel.text = "ECHO MASTERY";
        ui.progressLabel = Label(challenge, "Challenge Progress", new Vector2(0, -12), new Vector2(330, 23), 15);

        var backdrop = Stretch(root.transform, "Results Backdrop");
        backdrop.SetAsFirstSibling();
        backdrop.gameObject.AddComponent<Image>().color = new Color(0.015f, 0.02f, 0.04f, 0.9f);
        ui.resultsBackdrop = backdrop.gameObject;
        var card = Rect(ui.safeArea, "Results", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(650, 480));
        ui.resultsCard = card;
        card.gameObject.AddComponent<Image>().color = new Color(0.065f, 0.085f, 0.14f);
        var title = Label(card, "Title", new Vector2(0, 197), new Vector2(600, 48), 32);
        title.text = "Level complete!";
        title.color = new Color(0.35f, 0.9f, 1f);
        ui.levelLabel = Label(card, "Level", new Vector2(0, 158), new Vector2(600, 34), 22);
        ui.summaryLabel = Label(card, "Run Summary", new Vector2(0, 93), new Vector2(600, 88), 22);
        ui.badgesLabel = Label(card, "Badges", new Vector2(0, -12), new Vector2(600, 120), 22);
        ui.recordLabel = Label(card, "Saved Records", new Vector2(0, -111), new Vector2(600, 58), 18);
        foreach (Text label in card.GetComponentsInChildren<Text>())
        {
            var labelRect = label.rectTransform;
            float height = labelRect.sizeDelta.y;
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(1, 0.5f);
            labelRect.sizeDelta = new Vector2(-32, height);
        }
        ui.continueButton = Button(card, "Continue", new Vector2(-186, -193));
        ui.replayButton = Button(card, "Replay", new Vector2(0, -193));
        ui.menuButton = Button(card, "Main Menu", new Vector2(186, -193));
        // Results use a separate child because the safe-area card must sit above the backdrop.
        card.gameObject.SetActive(false);
        backdrop.gameObject.SetActive(false);
        root.SetActive(true);
        MobileControlsUI.EnsureEventSystem();
        return ui;
    }

    private static RectTransform Stretch(Transform parent, string name)
    {
        var rect = Rect(parent, name, Vector2.zero, Vector2.zero, Vector2.zero);
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        return rect;
    }

    private static RectTransform Rect(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }

    private static Text Label(Transform parent, string name, Vector2 position, Vector2 size, int fontSize)
    {
        var rect = Rect(parent, name, new Vector2(0.5f, 0.5f), position, size);
        var text = rect.gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private static Button Button(Transform parent, string title, Vector2 position)
    {
        var rect = Rect(parent, title, new Vector2(0.5f, 0.5f), position, new Vector2(156, 56));
        var image = rect.gameObject.AddComponent<Image>();
        image.color = new Color(0.12f, 0.28f, 0.4f);
        var button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        Label(rect, "Label", Vector2.zero, new Vector2(150, 50), 21).text = title;
        return button;
    }
}
