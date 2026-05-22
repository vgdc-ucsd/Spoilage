using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A set of semi-key characters. One is rolled at the start of a
/// run, and its members may be scheduled on the timeline for that run.
/// </summary>
[Serializable]
public class SemiKeySet
{
    /// <summary>Identifier for this set.</summary>
    public string id;

    /// <summary>Characters that may appear during a run when this set is active.</summary>
    public List<CustomerData> members = new List<CustomerData>();
}

/// <summary>
/// One option in the additional-character roll (Propagandist / Billman), or neither when
/// <see cref="character"/> is left empty.
/// </summary>
[Serializable]
public class AdditionalCharacterEntry
{
    /// <summary>Character to add on top of the chosen set, or null for "neither".</summary>
    public CustomerData character;

    /// <summary>Relative pick weight, where higher values are more likely to win the roll.</summary>
    [Min(0f)] public float weight = 1f;
}

/// <summary>
/// Catalog of every pre-made character that can appear in a run, partitioned
/// by tier and by which selection mechanism brings them in (semi-key set
/// roll, additional-character roll, day-scheduled key slot).
/// </summary>
/// <remarks>
/// To add a new character, create a <see cref="CustomerData"/> asset
/// (set <see cref="CustomerData.tier"/> and <see cref="CustomerData.id"/>),
/// drag it into the appropriate list here, and schedule it on a
/// <see cref="DayTimeline"/>. Multiple appearances of the same character are
/// represented by multiple <see cref="CustomerData"/> assets sharing one
/// <see cref="CustomerData.id"/>.
/// </remarks>
[CreateAssetMenu(fileName = "CharacterRoster", menuName = "Story/Character Roster", order = 1)]
public class CharacterRoster : ScriptableObject
{
    /// <summary>
    /// Key characters available to the run. Each is wired into a specific
    /// <see cref="DayEntry.beginInteraction"/> or
    /// <see cref="DayEntry.endInteraction"/> slot on the timeline.
    /// </summary>
    public List<CustomerData> keyCharacters = new List<CustomerData>();

    /// <summary>
    /// The three semi-key sets. One is rolled at the start of a run and its
    /// members become eligible to be scheduled.
    /// </summary>
    public List<SemiKeySet> semiKeySets = new List<SemiKeySet>();

    /// <summary>
    /// Optional additional semi-key characters (Propagandist / Billman /
    /// Neither) layered on top of the rolled set. Weights are relative.
    /// </summary>
    public List<AdditionalCharacterEntry> additionalCharacters = new List<AdditionalCharacterEntry>();
}
