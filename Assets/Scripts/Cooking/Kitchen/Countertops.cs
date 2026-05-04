using UnityEngine;

public class Countertops : CookingStation
{
    public override void OnPlaceFood(FoodGrab food)
    {
        base.OnPlaceFood(food);

        Debug.Log("Food on Countertop");

        if (_currentFood.IngredientInstance.IsSpoiled)
        {
            Debug.Log("Food is already spoiled");
            return;
        }

        if (_currentFood.IngredientInstance == null || _currentFood.IngredientInstance.Data == null)
        {
            Debug.LogError("Food data is missing on " + _currentFood.name);
            return;
        }

        _currentIngredientBehaviour.PutOnSpoilSurface();
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
        _currentFood = null;
    }
}
