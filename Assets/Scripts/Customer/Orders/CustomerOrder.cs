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



/// Notes to myself:
/// - There is a confusion between class Dish from the cooking team and class CustomerOrder from the customer team. Right now
/// the CustomerOrder ScriptableObject is being used to represent the a dish that the customer orders, but the cooking team has 
/// a Dish class that is being used to represent the dish that the player is cooking. They should match up, but they are currently separate. 
/// We should probably merge them at some point to avoid confusion.
/// - 