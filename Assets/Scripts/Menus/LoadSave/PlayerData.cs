using System;
using System.Collections.Generic;

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
