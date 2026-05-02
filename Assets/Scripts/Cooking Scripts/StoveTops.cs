using UnityEngine;

public class StoveTops : CookingAppliance
{

    private IngredientObject _currentFood;

    public override void OnPlaceFood(FoodGrab food)
    {
        _currentFood = food.GetComponent<IngredientObject>();

        IngredientBehaviour ingredientBehaviour = food.GetComponent<IngredientBehaviour>();

        // SAFETY CHECK: Ensure food and timer exist
        if (_currentFood == null || ingredientBehaviour == null)
        {
            Debug.LogWarning("Missing IngredientObject or Timer reference!");
            return;
        }

        // SAFETY CHECK: Ensure the ingredient data was assigned in the Inspector
        if (_currentFood.IngredientInstance == null || _currentFood.IngredientInstance.Data == null)
        {
            Debug.LogError("Food data is missing on " + _currentFood.name);
            return;
        }

        Debug.Log("Food on Grill");

        ingredientBehaviour.PutOnHeat();
    }


    public override void OnRemoveFood()
    {
        if (_currentFood == null)
        {
            return;
        }

        IngredientBehaviour ingredientBehaviour = _currentFood.GetComponent<IngredientBehaviour>();

        if (ingredientBehaviour != null)
        {
            ingredientBehaviour.RemoveFromHeat();
            
        }
        Debug.Log("Food removed from stove");
        _currentFood = null;
    }

}
