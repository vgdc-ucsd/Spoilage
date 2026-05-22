using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Outcome that triggers a particular ending screen.</summary>
public enum EndingKind
{
    WarlordDishA,
    WarlordDishAFailed,
    WarlordDishB,
    WarlordDishBFailed,
    WarlordDishC,
    WarlordDishCFailed,
    MoneyLoss,
    ReputationLoss,
    KilledByCustomer
}

/// <summary>
/// One ending screen, including its art and the dialogue shown over it.
/// </summary>
[Serializable]
public class EndingEntry
{
    public string id;
    public EndingKind kind;

    /// <summary>Splash art shown on the ending screen.</summary>
    public Sprite art;

    /// <summary>
    /// Dialogue key for the lines played over the art.
    /// </summary>
    public string dialogueKey;
}

/// <summary>
/// All ending screens: the warlord-encounter outcomes on day 30 and the
/// early-loss screens (money, reputation, killed by customer).
/// </summary>
[CreateAssetMenu(fileName = "Endings", menuName = "Story/Endings", order = 5)]
public class Endings : ScriptableObject
{
    public List<EndingEntry> entries = new List<EndingEntry>();
}
