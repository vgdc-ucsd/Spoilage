using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Everything scheduled to happen on one specific day (1-30). One of these
/// per day in <see cref="DayTimeline.days"/>.
/// </summary>
/// <remarks>
/// All character fields are optional. A day with only <see cref="semiKeys"/>
/// populated runs a normal cooking phase with random fill plus the listed
/// semi-key characters. Beginning- and end-of-day interaction slots are
/// skipped when their character is null. Radio is not scheduled here, see
/// <see cref="RadioBroadcasts"/>.
/// </remarks>
[Serializable]
public class DayEntry
{
    [Min(1)] public int day = 1;

    /// <summary>
    /// Character (key or semi-key) who appears before the cooking stage.
    /// </summary>
    public CustomerData beginInteraction;

    /// <summary>
    /// Character (key or semi-key) who walks up after the cooking phase,
    /// before the end-of-day screen.
    /// </summary>
    public CustomerData endInteraction;

    /// <summary>Semi-key characters guaranteed to appear during cooking on this day.</summary>
    public List<CustomerData> semiKeys = new List<CustomerData>();

    /// <summary>
    /// Pairs of semi-keys that must appear immediately back-to-back, in the
    /// listed order (e.g. the Unlucky Twins).
    /// </summary>
    public List<SemiKeyPair> semiKeyPairs = new List<SemiKeyPair>();
}

[Serializable]
public class SemiKeyPair
{
    public CustomerData first;
    public CustomerData second;
}

/// <summary>
/// The 30-day story schedule. A <see cref="DayEntry"/> spells out
/// scheduled key/semi-key appearances and beginning- and end-of-day
/// interactions.
/// </summary>
/// <remarks>
/// The list is sparse, each entry carries its own <see cref="DayEntry.day"/>,
/// and the manager looks them up by number. If a day has nothing interesting,
/// it can be omitted for the standard random behavior.
/// </remarks>
[CreateAssetMenu(fileName = "DayTimeline", menuName = "Story/Day Timeline", order = 2)]
public class DayTimeline : ScriptableObject
{
    public List<DayEntry> days = new List<DayEntry>();
}
