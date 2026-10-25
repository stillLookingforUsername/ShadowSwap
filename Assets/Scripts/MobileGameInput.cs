using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Touch input shared by custom buttons and the default phone layout.</summary>
public static class MobileGameInput
{
    public enum Control
    {
        MoveLeft, MoveRight, ClimbUp, ClimbDown,
        Jump, Swap, Interact, SmallBox, BigBox, Pause
    }

    public static event Action<Control> ActionPressed;
    public static event Action ResetRequested;

    // Each button owns its direction. Releasing one button never releases another.
    private static readonly Dictionary<int, Control> heldDirections = new Dictionary<int, Control>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        heldDirections.Clear();
        ActionPressed = null;
        ResetRequested = null;
    }

    public static bool IsDirection(Control control)
    {
        return control == Control.MoveLeft || control == Control.MoveRight ||
               control == Control.ClimbUp || control == Control.ClimbDown;
    }

    public static void Hold(int owner, Control control)
    {
        if (Time.timeScale > 0f && IsDirection(control)) heldDirections[owner] = control;
    }

    public static void Release(int owner) => heldDirections.Remove(owner);

    public static void Press(Control control)
    {
        if (!IsDirection(control) && (Time.timeScale > 0f || control == Control.Pause))
            ActionPressed?.Invoke(control);
    }

    public static float ResolveHorizontal(float otherInput) => ResolveAxis(otherInput, Control.MoveLeft, Control.MoveRight);
    public static float ResolveVertical(float otherInput) => ResolveAxis(otherInput, Control.ClimbDown, Control.ClimbUp);

    private static float ResolveAxis(float otherInput, Control negative, Control positive)
    {
        bool negativeHeld = false;
        bool positiveHeld = false;
        foreach (Control control in heldDirections.Values)
        {
            negativeHeld |= control == negative;
            positiveHeld |= control == positive;
        }

        if (!negativeHeld && !positiveHeld) return otherInput;
        return (positiveHeld ? 1f : 0f) - (negativeHeld ? 1f : 0f);
    }

    public static void Clear()
    {
        heldDirections.Clear();
        ResetRequested?.Invoke();
    }
}
