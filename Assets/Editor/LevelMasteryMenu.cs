using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class LevelMasteryMenu
{
    [MenuItem("GameObject/ShadowSwap/Echo Mastery", false, 12)]
    private static void CreateMastery()
    {
        var existing = Object.FindFirstObjectByType<LevelMasteryUI>(FindObjectsInactive.Include);
        if (existing)
        {
            Selection.activeGameObject = existing.gameObject;
            return;
        }
        var player = Object.FindFirstObjectByType<PlayerMovement2D>();
        if (!player)
        {
            Debug.Log("Open a gameplay scene with a player to add Echo Mastery.");
            return;
        }
        var tracker = player.GetComponent<LevelRunTracker>();
        if (!tracker) tracker = Undo.AddComponent<LevelRunTracker>(player.gameObject);
        bool hadEvents = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null;
        var ui = LevelMasteryUI.Create(tracker);
        Undo.RegisterCreatedObjectUndo(ui.gameObject, "Create Echo Mastery");
        if (!hadEvents)
        {
            var events = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (events) Undo.RegisterCreatedObjectUndo(events.gameObject, "Create EventSystem");
        }
        Selection.activeGameObject = ui.gameObject;
        EditorSceneManager.MarkSceneDirty(ui.gameObject.scene);
    }
}
