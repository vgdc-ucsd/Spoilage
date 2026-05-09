using UnityEngine;

public class Countertops : CookingStation
{
    public override void OnPlaceFood(FoodGrab food)
    {
        base.OnPlaceFood(food);

        Debug.Log("Food on Countertop");

        if (_currentFood == null || _currentFood.IngredientInstance == null)
        {
            Debug.LogWarning("Missing IngredientObject.");
            return;
        }

        if (_currentFood.IngredientInstance.IsSpoiled)
        {
            Debug.Log("Food is already spoiled");
            return;
        }

        if (_currentIngredientBehaviour != null)
        {
            _currentIngredientBehaviour.PutOnSpoilSurface();
        }
    }

    public override void OnRemoveFood()
    {
        if (_currentFood == null)
        {
            return;
        }

        if (_currentIngredientBehaviour != null)
        {
            _currentIngredientBehaviour.RemoveFromSpoilSurface();
        }

        Debug.Log("Food removed from counter");

        base.OnRemoveFood();
    }
}