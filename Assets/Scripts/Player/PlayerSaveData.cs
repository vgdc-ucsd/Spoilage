using System.Collections.Generic;
using NUnit.Framework;

[System.Serializable]
public class PlayerSaveData
{
    public int customersServed;
    public int moneyGained;
    public int spoiledRejected;
    public int healthyRejected;
    public int dayCount;
    public List<int> ingredientsGathered;  // List of ingredient names as ints using IngredientType enum
}
