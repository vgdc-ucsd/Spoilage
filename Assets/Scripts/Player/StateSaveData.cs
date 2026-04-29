using System.Collections.Generic;
using NUnit.Framework;

[System.Serializable]

public class IngredientSaveData
{
    // The string name acts as your "ID" so you know what this is later
    public string ingredientName;

    // Dynamic states specific to THIS exact piece of food
    public StateSaveData ingredientState;
    public int currentCookTime;
    public int currentSpoilTime;

    public IngredientSaveData(string name, StateSaveData state, int cookTime, int spoilTime)
    {
        ingredientName = name;
        ingredientState = state;
        currentCookTime = cookTime;
        currentSpoilTime = spoilTime;
    }
}
public class StateSaveData
{
    public List<int> stoveLayout;  // stove item layout as ints using IngredientType enum
                                   // list of 10 or 12 items, with -1 representing empty slots and 0-5 as first row

    public List<IngredientSaveData> playerIngredients;
}
