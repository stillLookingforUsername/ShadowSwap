using UnityEngine;

/// <summary>Stores the player's motion preference locally.</summary>
public static class ShadowFeedbackSettings
{
    private const string ReducedMotionKey = "ShadowSwap.ReducedMotion";
    private static bool initialized;
    private static bool reducedMotion;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset() => initialized = false;

    public static bool ReducedMotion
    {
        get
        {
            if (!initialized)
            {
                reducedMotion = PlayerPrefs.GetInt(ReducedMotionKey, 0) != 0;
                initialized = true;
            }
            return reducedMotion;
        }
        set
        {
            if (ReducedMotion == value) return;
            reducedMotion = value;
            PlayerPrefs.SetInt(ReducedMotionKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
