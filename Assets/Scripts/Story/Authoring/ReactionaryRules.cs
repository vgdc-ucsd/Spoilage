using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One conditional rule that schedules a reactionary character for the
/// following day when its <see cref="condition"/> holds at end-of-day.
/// </summary>
[Serializable]
public class ReactionaryRule
{
    /// <summary>
    /// Boolean expression evaluated against the current <see cref="PlayerData"/>.
    /// Null counts as always-true, which queues the spawn every day.
    /// </summary>
    [SerializeReference] public ConditionNode condition;

    /// <summary>Character added to the next day's pool when this rule fires.</summary>
    public CustomerData spawnsCharacter;
}

/// <summary>
/// All reactionary rules evaluated at end-of-day. A rule that fires queues
/// its <see cref="ReactionaryRule.spawnsCharacter"/> onto
/// <see cref="PlayerData.pendingReactionaryIds"/> for the next day's
/// customer pool.
/// </summary>
/// <remarks>
/// Multiple rules may fire on the same day, but the manager deduplicates by
/// character id so the same reactionary is never queued twice.
/// </remarks>
[CreateAssetMenu(fileName = "ReactionaryRules", menuName = "Story/Reactionary Rules", order = 4)]
public class ReactionaryRules : ScriptableObject
{
    public List<ReactionaryRule> rules = new List<ReactionaryRule>();
}
