using UnityEngine;

public class Countertops : CookingStation
{
    public override void OnPlaceFood(FoodGrab food)
    {
        if (!HasSpace)
        {
            Debug.LogWarning($"{gameObject.name}: Countertop is full.");
            return;
        }

        base.OnPlaceFood(food);

        Debug.Log("Food on Countertop");

        if (_currentFood == null || _currentFood.IngredientInstance == null)
        {
            Debug.LogWarning("Missing IngredientObject.");
            return;
        }

        if (_currentFood.IngredientInstance.IsSpoiled)
        {
            Debug.Log("Food is already spoiled.");
            return;
        }

        _currentIngredientBehaviour?.PutOnSpoilSurface();
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