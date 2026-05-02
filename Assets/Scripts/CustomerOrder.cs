using UnityEngine;

[CreateAssetMenu(fileName = "CustomerOrder", menuName = "ScriptableObjects/CustomerOrder", order = 2)]
public class CustomerOrder : ScriptableObject
{
    public enum Difficulty
    {
        EASY,
        MEDIUM,
        HARD
    }

    public string DishName;
    public Ingredient[] Ingredients;
    public Difficulty difficulty;

    //  We'll need a function somewhere to check if the player has the ingredients for this order. 
    //  Maybe in the CustomerManager or maybe in a separate InventoryManager but that's another subteam's responsibility
    //  For now I'll have a placeholder function that always returns true

    public bool CheckPlayerHasIngredients()
    {
        return true;
    }
}
