using System;
using UnityEngine;

/// <summary>Independent, per-level records; this never changes the campaign score.</summary>
public static class LevelMasterySave
{
    private const string Prefix = "ShadowSwap.Mastery.v1.";

    public static LevelMasteryRecord Load(string levelId)
    {
        string json = PlayerPrefs.GetString(Prefix + levelId, "");
        if (string.IsNullOrEmpty(json)) return new LevelMasteryRecord();
        try
        {
            var record = JsonUtility.FromJson<LevelMasteryRecord>(json);
            if (record == null || record.version != 1 || !record.completed ||
                record.bestSeconds < 0f || float.IsNaN(record.bestSeconds) || float.IsInfinity(record.bestSeconds) ||
                record.completions < 1) return new LevelMasteryRecord();
            record.bestPickups = Mathf.Max(0, record.bestPickups);
            record.fewestSwaps = Mathf.Max(0, record.fewestSwaps);
            record.badges &= LevelMasteryBadge.CleanRun | LevelMasteryBadge.Collector | LevelMasteryBadge.ShadowDancer;
            return record;
        }
        catch (ArgumentException)
        {
            return new LevelMasteryRecord();
        }
    }

    public static LevelMasteryCompletion SaveCompletion(string levelId, LevelRunSummary summary)
    {
        if (string.IsNullOrWhiteSpace(levelId)) throw new ArgumentException("A level ID is required.", nameof(levelId));
        if (summary == null) throw new ArgumentNullException(nameof(summary));
        if (summary.ElapsedSeconds < 0f || float.IsNaN(summary.ElapsedSeconds) || float.IsInfinity(summary.ElapsedSeconds) ||
            summary.Deaths < 0 || summary.Pickups < 0 || summary.TotalPickups < summary.Pickups ||
            summary.Swaps < summary.AirSwaps || summary.AirSwaps < 0 || summary.AirSwapGoal < 1)
            throw new ArgumentException("Attempt values must be finite and consistent.", nameof(summary));
        var record = Load(levelId);
        bool firstClear = !record.completed;
        bool personalBest = firstClear || summary.ElapsedSeconds < record.bestSeconds;
        LevelMasteryBadge newBadges = summary.EarnedBadges & ~record.badges;
        if (personalBest) record.bestSeconds = summary.ElapsedSeconds;
        record.bestPickups = Mathf.Max(record.bestPickups, summary.Pickups);
        record.fewestSwaps = firstClear ? summary.Swaps : Mathf.Min(record.fewestSwaps, summary.Swaps);
        record.badges |= summary.EarnedBadges;
        record.completions++;
        record.completed = true;
        PlayerPrefs.SetString(Prefix + levelId, JsonUtility.ToJson(record));
        PlayerPrefs.Save();
        return new LevelMasteryCompletion(summary, record, newBadges, firstClear, personalBest);
    }
}
