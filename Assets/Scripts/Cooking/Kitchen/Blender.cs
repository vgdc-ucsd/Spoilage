using UnityEngine;
using System.Collections.Generic;

public class Blender : ManualStation
{
    public override void Start()
    {
        maxIngredients = 3;
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

        foreach (IngredientObject existing in _currentFoods)
        {
            if (existing != null &&
                existing.IngredientInstance.Data == incoming.IngredientInstance.Data)
            {
                Debug.Log($"{gameObject.name}: Duplicate ingredient rejected.");
                return false;
            }
        }

        bool placed = base.OnPlaceFood(food);

        // reset blender clicks every time a new ingredient is added
        _currentClicks = 0;
        _isActionComplete = false;
        UpdateTimer();

        RecipeManager recipeManager = FindAnyObjectByType<RecipeManager>();
        if (recipeManager == null)
        {
            Debug.LogError($"{gameObject.name}: RecipeManager not found.");
            return false;
        }

        string result = recipeManager.CheckRecipe(_currentFoods, _station);

        if (IsInvalidRecipeResult(result))
        {
            Debug.Log($"{gameObject.name}: Current blender ingredients do not make a recipe yet.");
            return true;
        }

        Debug.Log($"{gameObject.name}: Valid blender recipe found: {result}");

        return placed;
    }

    public void PressBlendButton()
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

        string resultName = recipeManager.CheckRecipe(_currentFoods, _station);

        if (IsInvalidRecipeResult(resultName))
        {
            Debug.Log($"{gameObject.name}: Cannot blend. Invalid recipe.");
            ResetTimer();
            return;
        }

        IngredientData resultData = IngredientLookup.Get(resultName);

        if (resultData == null)
        {
            Debug.LogError($"{gameObject.name}: Could not find IngredientData for result '{resultName}'.");
            ResetTimer();
            return;
        }

        IngredientObject survivor = _currentFoods[0];
        survivor.ChangeIngredient(resultData);

        DestroyExtraIngredients();

        _currentFoods.Clear();
        _currentBehaviours.Clear();
        _currentFoods.Add(survivor);

        Debug.Log($"{gameObject.name}: Blended! → {resultData.Name}");
    }

    private void DestroyExtraIngredients()
    {
        for (int i = 1; i < _currentFoods.Count; i++)
        {
            if (_currentFoods[i] != null)
            {
                Destroy(_currentFoods[i].gameObject);
            }
        }
    }

    private bool IsInvalidRecipeResult(string resultName)
    {
        return string.IsNullOrEmpty(resultName)
            || resultName == "Slop"
            || resultName == "JSON Error";
    }
}