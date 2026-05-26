using UnityEngine;
using System.Collections.Generic;

public class CuttingBoard : ManualStation
{
    public override void Start()
    {
        maxIngredients = 1;
        base.Start();
    }

    public override bool OnPlaceFood(FoodGrab food)
    {
        IngredientObject incoming = food.GetComponent<IngredientObject>();
        if (incoming == null) return false;

        if (_currentFoods.Contains(incoming))
            return true;

        if (incoming.IngredientInstance.Data.Name == RecipeManager.SlopResult)
            return false;

        if (!HasSpace)
        {
            Debug.LogWarning($"{gameObject.name}: Cutting board only accepts one ingredient.");
            return false;
        }

        bool placed = base.OnPlaceFood(food);

        if (_currentFood == null || _currentFood.IngredientInstance == null)
            return false;

        RecipeManager recipeManager = FindAnyObjectByType<RecipeManager>();
        if (recipeManager == null)
        {
            Debug.LogError($"{gameObject.name}: RecipeManager not found.");
            return false;
        }

        Debug.Log($"{gameObject.name}: Food on cutting board.");

        return placed;
    }

    public void PressCutButton()
    {
        Debug.Log($"{gameObject.name}: Cut button pressed.");
        OnAction();
    }

    protected override void CompleteManualAction()
    {
        RecipeManager recipeManager = FindAnyObjectByType<RecipeManager>();
        if (recipeManager == null)
        {
            Debug.LogError($"{gameObject.name}: RecipeManager not found.");
            ResetTimer();
            return;
        }

        List<IngredientObject> ingredients = new() { _currentFood };
        string resultName = recipeManager.CheckRecipe(ingredients, _station);

        IngredientData resultData = IngredientLookup.Get(resultName);

        if (resultData == null)
        {
            Debug.LogError($"{gameObject.name}: Could not find IngredientData for result '{resultName}'.");
            ResetTimer();
            return;
        }

        bool usedUnspoiledFood = SpoilageTriggerManager.IsUnspoiledFood(_currentFood);

        _currentFood.ChangeIngredient(resultData);

        SpoilageTriggerManager.TriggerIf(SpoilageCategory.DISGUST, usedUnspoiledFood);

        Debug.Log($"{gameObject.name}: Chopped! → {resultData.Name}");
    }
}