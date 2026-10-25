using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MobileControlsMenu
{
    [MenuItem("GameObject/ShadowSwap/Mobile Controls", false, 10)]
    private static void CreateControls()
    {
        var existing = Object.FindFirstObjectByType<MobileControlsUI>(FindObjectsInactive.Include);
        if (existing)
        {
            Selection.activeGameObject = existing.gameObject;
            return;
        }

        var ui = MobileControlsUI.CreateDefault();
        Undo.RegisterCreatedObjectUndo(ui.gameObject, "Create Mobile Controls");
        if (!Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>())
        {
            MobileControlsUI.EnsureEventSystem();
            var events = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            Undo.RegisterCreatedObjectUndo(events.gameObject, "Create EventSystem");
        }
        Selection.activeGameObject = ui.gameObject;
        EditorSceneManager.MarkSceneDirty(ui.gameObject.scene);
    }
}
