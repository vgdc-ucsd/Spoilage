using System;
using System.Collections.Generic;

/// <summary>
/// A record for an item changing hands during the run.
/// </summary>
[Serializable]
public class ItemExchange
{
    /// <summary>Matches a <see cref="StoryItemEntry.id"/>.</summary>
    public string itemId;

    /// <summary>Matches a <see cref="CustomerData.id"/>.</summary>
    public string recipientId;
}

[Serializable]
public class PlayerData
{
    public int Day;
    public int Wealth;
    public int Reputation;

    // TODO: Handle saving other key information
    // hi :) Currently the stations and ingredients are just string lists, this
    // is just a temporary solution for the purposes of order generation. Feel free 
    // to change it to whatever format you want as long as the name can still be
    // easily accessed - Samantha M
    public List<Recipe> RecipesUnlocked;
    public List<string> StationsUnlocked;
    public List<string> IngredientsUnlocked;
    // public List<UpgradeID> Upgrades; 
    // public List<NPCID> NPCs; 
    // public List<PlotEventID> PlotEvents; 
    // public List<StationID> KitchenLayout;

    /// <summary>
    /// Player resistance. Below 7 leans warlord, above 7 leans
    /// resistance.
    /// </summary>
    public float resistanceScore = 7f;

    /// <summary>Id of the active <see cref="SemiKeySet"/> for this run.</summary>
    public string activeSetId;

    /// <summary>Id of the additional semi-key character for this run, or empty for none.</summary>
    public string activeAdditionalId;

    /// <summary>
    /// Character ids that have been refused at least once this run. No future
    /// appearance of any character on this list will be scheduled.
    /// </summary>
    public List<string> refusedCharacterIds = new List<string>();

    /// <summary>Character ids that have been successfully served at least once.</summary>
    public List<string> servedCharacterIds = new List<string>();

    public int semiKeyRefusedToday;
    public int semiKeyRefusedLifetime;

    public int semiImportantDishesFailedToday;
    public int semiImportantDishesFailedLifetime;

    /// <summary>Story items the player has handed to characters this run.</summary>
    public List<ItemExchange> itemExchanges = new List<ItemExchange>();

    /// <summary>Reactionary character ids queued for today's customer pool.</summary>
    public List<string> pendingReactionaryIds = new List<string>();

    public PlayerData()
    {
        // TODO: Initialize lists, setup other basic start of game configs
        // Day = 1;
        // Reputation = 50;
        RecipesUnlocked = new();

        // Initialize StationsUnlocked and IngredientsUnlocked with the day 1 status
        StationsUnlocked = new()
        {
            "Grill",
        };
        IngredientsUnlocked = new()
        {
            "Dough"
        };
    }
}
