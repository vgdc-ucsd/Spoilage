using UnityEngine;

public class Countertops : CookingStation
{
    public override bool OnPlaceFood(FoodGrab food)
    {
        if (!HasSpace)
        {
            Debug.LogWarning($"{gameObject.name}: Countertop is full.");
            return false;
        }

        base.OnPlaceFood(food);

        Debug.Log("Food on Countertop");

        if (_currentFood == null || _currentFood.IngredientInstance == null)
        {
            Debug.LogWarning("Missing IngredientObject.");
            return false;
        }

        if (_currentFood.IngredientInstance.IsSpoiled)
        {
            Debug.Log("Food is already spoiled.");
            return false;
        }

        _currentIngredientBehaviour?.PutOnSpoilSurface();
        return true;
    }

    public override void OnRemoveFood()
    {
        if (_currentFood == null)
        {
            return;
        }

        _currentIngredientBehaviour?.RemoveFromSpoilSurface();

        Debug.Log("Food removed from counter.");

        base.OnRemoveFood();
    }
}