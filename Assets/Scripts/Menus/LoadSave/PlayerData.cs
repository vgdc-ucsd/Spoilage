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
    public DayData DayData;

    // TODO: Handle saving other key information
    // hi :) Currently the stations and ingredients are just string lists, this
    // is just a temporary solution for the purposes of order generation. Feel free 
    // to change it to whatever format you want as long as the name can still be
    // easily accessed - Samantha M
    public List<Recipe> RecipesUnlocked;
    public List<string> StationsUnlocked;
    public List<string> IngredientsUnlocked;
    
    public List<string> KitchenStations;
    public List<string> KitchenItems;
    public string PendingStation;
    public GameOverCondition GameOverCondition;

    public List<string> SeenSemikeyCharacters;
    public List<string> RejectedSemikeyCharacters;

    /// <summary>
    /// Player resistance. Below 7 leans warlord, above 7 leans
    /// resistance.
    /// </summary>
    public float resistanceScore = 7f;

    public PlayerData()
    {
        // TODO: Initialize lists, setup other basic start of game configs
        // Reputation = 50;
        
        Day = 1;
        Wealth = 100;
        RecipesUnlocked = new();
        KitchenStations = new List<string>();
        KitchenItems = new List<string>();
        PendingStation = "Grill";
        RejectedSemikeyCharacters = new List<string>();
        SeenSemikeyCharacters = new List<string>();

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
