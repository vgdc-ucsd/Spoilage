using System;
using System.Collections.Generic;
using UnityEngine;

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
    public int SaveID;
    public string SaveName;
    public int Day;
    public int Wealth;
    public int Reputation;
    public int ReputationMax;
    public DayData DayData;

    // TODO: Handle saving other key information
    // hi :) Currently the stations and ingredients are just string lists, this
    // is just a temporary solution for the purposes of order generation. Feel free 
    // to change it to whatever format you want as long as the name can still be
    // easily accessed - Samantha M
    public List<string> StationsUnlocked;
    public List<string> IngredientsUnlocked;
    
    public List<string> KitchenStations;
    public List<string> KitchenItems;
    public string PendingStation;
    public GameOverCondition GameOverCondition;
    public GameEndScenario GameEndScenario;
    public GameEndItemServed GameEndItemServed;

    public List<string> SeenSemikeyCharacters;
    public List<string> RejectedSemikeyCharacters;

    /// <summary>
    /// Player resistance. Below 7 leans warlord, above 7 leans
    /// resistance.
    /// </summary>
    public float resistanceScore = 7f;

    // Graphs & Timelines
    public List<int?> InteractionNodes;
    public int? RadioNode;
    public int? UpgradeNode;

    // Shop, upgrades, and unlocks
    public List<UpgradeID> ShopPool;
    public List<UpgradeID> StationQueue;
    public List<UpgradeID> Unlocked;
    public List<UpgradeID> Purchased;

    public PlayerData()
    {   
        Day = 1;
        Wealth = 200;
        Reputation = 25;
        ReputationMax = 50;
        KitchenStations = new List<string>();
        KitchenItems = new List<string>();
        PendingStation = "Grill";
        RejectedSemikeyCharacters = new List<string>();
        SeenSemikeyCharacters = new List<string>();
        ShopPool = new List<UpgradeID>();
        StationQueue = new List<UpgradeID>();
        Unlocked = new List<UpgradeID>();
        Purchased = new List<UpgradeID>();
        DayData = new DayData();
        InteractionNodes = new List<int?>();

        // Initialize StationsUnlocked and IngredientsUnlocked with the day 1 status
        StationsUnlocked = new()
        {
            "Kitchen Tile",
            "Grill",
        };
        IngredientsUnlocked = new()
        {
            "Dough",
        };
    }

    public PlayerData Clone()
    {
        return JsonUtility.FromJson<PlayerData>(JsonUtility.ToJson(this));
    }
}
