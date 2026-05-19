using UnityEngine;
using System.Collections.Generic;

public class Blender : ManualStation
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

        if (incoming.IngredientInstance.Data.Name == "Slop")
            return false;

        if (!HasSpace)
        {
            Debug.LogWarning($"{gameObject.name}: Blender only accepts three ingredients max.");
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

        List<IngredientObject> check = new() { _currentFood };
        string result = recipeManager.CheckRecipe(check, _station);

        if (IsInvalidRecipeResult(result))
        {
            Debug.Log($"{gameObject.name}: Wrong ingredient for blender.");
            HideManualUI();
            return true;
        }

        Debug.Log($"{gameObject.name}: Food in blender.");

        return placed;
    }

    public void PressCutButton()
    {
        Debug.Log($"{gameObject.name}: Blend button pressed.");
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

        _currentFood.ChangeIngredient(resultData);

        Debug.Log($"{gameObject.name}: Chopped! → {resultData.Name}");
    }

    private bool IsInvalidRecipeResult(string resultName)
    {
        return string.IsNullOrEmpty(resultName)
            || resultName == "Slop"
            || resultName == "JSON Error";
    }
}