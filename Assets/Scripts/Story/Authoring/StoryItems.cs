using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Authoring data for a story item (Rat, Coded Message, etc). Per-run
/// records of which items have been exchanged live on
/// <see cref="PlayerData.itemExchanges"/>.
/// </summary>
[Serializable]
public class StoryItemEntry
{
    /// <summary>Identifier referenced from <see cref="ItemExchange.itemId"/>.</summary>
    public string id;

    public Sprite icon;
    public string displayName;
}

/// <summary>
/// Catalog of all story items the player can give to or receive from
/// characters.
/// </summary>
[CreateAssetMenu(fileName = "StoryItems", menuName = "Story/Story Items", order = 6)]
public class StoryItems : ScriptableObject
{
    public List<StoryItemEntry> entries = new List<StoryItemEntry>();
}
