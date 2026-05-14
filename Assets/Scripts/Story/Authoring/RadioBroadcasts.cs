using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One radio broadcast. Eligibility on a given day is decided by
/// <see cref="condition"/>.
/// </summary>
[Serializable]
public class RadioEntry
{
    /// <summary>Dialogue key for the broadcast text.</summary>
    public string dialogueKey;

    /// <summary>True when this entry is part of the filler pool used for unscripted days.</summary>
    public bool isFiller;

    /// <summary>
    /// Boolean expression that decides whether this broadcast is eligible
    /// today. Null counts as always-eligible.
    /// </summary>
    [SerializeReference] public ConditionNode condition;
}

/// <summary>
/// All radio broadcasts available to a run, both day-scheduled and filler.
/// </summary>
/// <remarks>
/// Each end-of-day screen plays exactly one broadcast. The manager scans
/// non-filler entries first and picks the first one whose
/// <see cref="RadioEntry.condition"/> holds against the current
/// <see cref="PlayerData"/>, falling back to a randomly picked filler entry
/// if nothing matches.
/// </remarks>
[CreateAssetMenu(fileName = "RadioBroadcasts", menuName = "Story/Radio Broadcasts", order = 3)]
public class RadioBroadcasts : ScriptableObject
{
    public List<RadioEntry> entries = new List<RadioEntry>();
}
