using System;
using System.Collections.Generic;

[Flags]
public enum LevelMasteryBadge
{
    None = 0,
    CleanRun = 1,
    Collector = 2,
    ShadowDancer = 4
}

/// <summary>A single attempt; repeated pickup notifications cannot inflate collection.</summary>
public sealed class LevelAttempt
{
    private readonly HashSet<int> collectibleIds;
    private readonly HashSet<int> collectedIds = new HashSet<int>();
    public float ElapsedSeconds { get; private set; }
    public int Deaths { get; private set; }
    public int Swaps { get; private set; }
    public int AirSwaps { get; private set; }
    public int AirSwapGoal { get; }
    public int Pickups => collectedIds.Count;
    public int TotalPickups => collectibleIds.Count;
    public bool IsComplete { get; private set; }

    public LevelAttempt(IEnumerable<int> pickups, int airSwapGoal = 3)
    {
        collectibleIds = new HashSet<int>(pickups);
        AirSwapGoal = Math.Max(1, airSwapGoal);
    }

    public void Advance(float seconds)
    {
        if (!IsComplete && seconds > 0f && !float.IsNaN(seconds) && !float.IsInfinity(seconds))
            ElapsedSeconds += seconds;
    }

    public bool Collect(int id)
    {
        return !IsComplete && collectibleIds.Contains(id) && collectedIds.Add(id);
    }

    public void RecordDeath()
    {
        if (!IsComplete) Deaths++;
    }

    public void RecordSwap(bool wasAirborne)
    {
        if (IsComplete) return;
        Swaps++;
        if (wasAirborne) AirSwaps++;
    }

    public LevelRunSummary Complete()
    {
        IsComplete = true;
        return new LevelRunSummary(ElapsedSeconds, Deaths, Pickups, TotalPickups, Swaps, AirSwaps, AirSwapGoal);
    }
}

public sealed class LevelRunSummary
{
    public float ElapsedSeconds { get; }
    public int Deaths { get; }
    public int Pickups { get; }
    public int TotalPickups { get; }
    public int Swaps { get; }
    public int AirSwaps { get; }
    public int AirSwapGoal { get; }
    public LevelMasteryBadge AvailableBadges => LevelMasteryBadge.CleanRun | LevelMasteryBadge.ShadowDancer |
        (TotalPickups > 0 ? LevelMasteryBadge.Collector : LevelMasteryBadge.None);

    public LevelMasteryBadge EarnedBadges => (Deaths == 0 ? LevelMasteryBadge.CleanRun : LevelMasteryBadge.None) |
        (TotalPickups > 0 && Pickups >= TotalPickups ? LevelMasteryBadge.Collector : LevelMasteryBadge.None) |
        (AirSwaps >= AirSwapGoal ? LevelMasteryBadge.ShadowDancer : LevelMasteryBadge.None);

    public LevelRunSummary(float seconds, int deaths, int pickups, int total, int swaps, int airSwaps, int airSwapGoal)
    {
        ElapsedSeconds = seconds;
        Deaths = deaths;
        Pickups = pickups;
        TotalPickups = total;
        Swaps = swaps;
        AirSwaps = airSwaps;
        AirSwapGoal = airSwapGoal;
    }
}

[Serializable]
public sealed class LevelMasteryRecord
{
    public int version = 1;
    public bool completed;
    public float bestSeconds;
    public int bestPickups;
    public int fewestSwaps;
    public int completions;
    public LevelMasteryBadge badges;
}

public sealed class LevelMasteryCompletion
{
    public LevelRunSummary Summary { get; }
    public LevelMasteryRecord Record { get; }
    public LevelMasteryBadge NewBadges { get; }
    public bool FirstClear { get; }
    public bool PersonalBest { get; }

    public LevelMasteryCompletion(LevelRunSummary summary, LevelMasteryRecord record, LevelMasteryBadge newBadges, bool firstClear, bool personalBest)
    {
        Summary = summary;
        Record = record;
        NewBadges = newBadges;
        FirstClear = firstClear;
        PersonalBest = personalBest;
    }
}
