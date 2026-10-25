using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ShadowSenseMenu
{
    [MenuItem("GameObject/ShadowSwap/Shadow Sense", false, 11)]
    private static void CreateShadowSense()
    {
        var existing = Object.FindFirstObjectByType<ShadowSenseUI>(FindObjectsInactive.Include);
        if (existing)
        {
            Selection.activeGameObject = existing.gameObject;
            return;
        }
        var swap = Object.FindFirstObjectByType<ShadowSwap>();
        if (!swap)
        {
            Debug.Log("Open a gameplay scene with a ShadowSwap player to add Shadow Sense.");
            return;
        }
        bool hadEventSystem = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null;
        var ui = ShadowSenseUI.Create(swap);
        Undo.RegisterCreatedObjectUndo(ui.gameObject, "Create Shadow Sense");
        if (!hadEventSystem)
        {
            var events = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (events) Undo.RegisterCreatedObjectUndo(events.gameObject, "Create EventSystem");
        }
        if (!swap.GetComponent<ShadowSwapEcho>()) Undo.AddComponent<ShadowSwapEcho>(swap.gameObject);
        Selection.activeGameObject = ui.gameObject;
        EditorSceneManager.MarkSceneDirty(ui.gameObject.scene);
    }
}
