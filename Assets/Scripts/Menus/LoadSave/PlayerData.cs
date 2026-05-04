using System;

[Serializable]
public class PlayerData
{
    public int Day;
    public int Wealth;
    public int Reputation;

    // TODO: Handle saving other key information
    // public List<RecipeID> RecipesUnlocked;
    // public List<StationID> StationsUnlocked;
    // public List<IngredientID> IngredientsUnlocked;
    // public List<UpgradeID> Upgrades; 
    // public List<NPCID> NPCs; 
    // public List<PlotEventID> PlotEvents; 
    // public List<StationID> KitchenLayout;

    public PlayerData()
    {
        // TODO: Initialize lists, setup other basic start of game configs
        // Day = 1;
        // Reputation = 50;
        // RecipesUnlocked = new List<RecipeID>();
    }
}
