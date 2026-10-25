using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Add to a UI Image or Button and choose its Control in the Inspector.</summary>
[RequireComponent(typeof(RectTransform))]
public class MobileControlButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public MobileGameInput.Control control;
    public Graphic targetGraphic;
    public Color pressedColor = new Color(0.3f, 0.65f, 1f, 0.9f);

    private readonly HashSet<int> pointers = new HashSet<int>();
    private Color normalColor;

    private void Awake()
    {
        if (!targetGraphic) targetGraphic = GetComponent<Graphic>();
        if (targetGraphic) normalColor = targetGraphic.color;
    }

    private void OnEnable() => MobileGameInput.ResetRequested += ReleaseAll;

    private void OnDisable()
    {
        MobileGameInput.ResetRequested -= ReleaseAll;
        ReleaseAll();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isActiveAndEnabled) return;
        var button = GetComponent<Button>();
        if (button && !button.IsInteractable()) return;
        foreach (CanvasGroup group in GetComponentsInParent<CanvasGroup>())
        {
            if (!group.interactable) return;
            if (group.ignoreParentGroups) break;
        }
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (Time.timeScale <= 0f && control != MobileGameInput.Control.Pause) return;
        if (!pointers.Add(eventData.pointerId) || pointers.Count > 1) return;

        if (targetGraphic) targetGraphic.color = pressedColor;
        if (MobileGameInput.IsDirection(control)) MobileGameInput.Hold(GetInstanceID(), control);
        else MobileGameInput.Press(control);
    }

    public void OnPointerUp(PointerEventData eventData) => ReleasePointer(eventData.pointerId);
    public void OnPointerExit(PointerEventData eventData) => ReleasePointer(eventData.pointerId);

    private void ReleasePointer(int pointerId)
    {
        pointers.Remove(pointerId);
        if (pointers.Count == 0) ReleaseAll();
    }

    private void ReleaseAll()
    {
        pointers.Clear();
        MobileGameInput.Release(GetInstanceID());
        if (targetGraphic) targetGraphic.color = normalColor;
    }
}
