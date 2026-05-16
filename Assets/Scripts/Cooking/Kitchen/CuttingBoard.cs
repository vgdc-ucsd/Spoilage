using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CuttingBoard : ManualStation
{
    public override void Start()
    {
        maxIngredients = 1;
        base.Start();
    }

    private void Update()
    {
        if (_currentFood == null)
        {
            return;
        }

        // Temporary test input until popup button is implemented
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            OnAction();
        }
    }

    public override bool OnPlaceFood(FoodGrab food)
    {
        IngredientObject incoming = food.GetComponent<IngredientObject>();
        if (incoming == null) return false;

        if (_currentFoods.Contains(incoming))
        {
            return true; 
        }

        if (incoming.IngredientInstance.Data.Name == "Slop")
        {
            return false;
        }

        if (!HasSpace)
        {
            Debug.LogWarning($"{gameObject.name}: Cutting board only accepts one ingredient.");
            return false;
        }

        base.OnPlaceFood(food);

        if (_currentFood == null || _currentFood.IngredientInstance == null)
        {
            return false;
        }

        Debug.Log($"{gameObject.name}: Food on cutting board.");

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
            Debug.Log($"{gameObject.name}: Wrong ingredient for cutting board.");
        }
        return true;
    }

    public override void OnAction()
    {
        Debug.Log($"Action triggered on {gameObject.name}. Current Food: {(_currentFood != null ? _currentFood.name : "NULL")}");

        SpoilageTriggerManager.Instance.Invoke(SpoilageCategory.DISTRESS);

        if (_currentFood == null || _currentFood.IngredientInstance == null)
        {
            return;
        }

        base.OnAction();

        if (_currentClicks < _clicksPerState)
        {
            return;
        }

        RecipeManager recipeManager = FindAnyObjectByType<RecipeManager>();

        if (recipeManager == null)
        {
            Debug.LogError($"{gameObject.name}: RecipeManager not found.");
            _currentClicks = 0;
            return;
        }

        List<IngredientObject> ingredients = new() { _currentFood };
        string resultName = recipeManager.CheckRecipe(ingredients, _station);

        if (IsInvalidRecipeResult(resultName))
        {
            Debug.Log($"{gameObject.name}: Wrong ingredient for cutting board.");
            _currentClicks = 0;
            return;
        }

        IngredientData resultData = IngredientLookup.Get(resultName);

        if (resultData == null)
        {
            Debug.LogError($"{gameObject.name}: Could not find IngredientData for result '{resultName}'.");
            _currentClicks = 0;
            return;
        }

        _currentFood.ChangeIngredient(resultData);

        Debug.Log($"{gameObject.name}: Chopped! → {resultData.Name}");

        //reset cooking timer
        FoodGrab foodGrab = _currentFood.GetComponent<FoodGrab>();
        if (foodGrab != null)
            foodGrab.ClearCookTimers();

        _currentClicks = 0;
    }

    private bool IsInvalidRecipeResult(string resultName)
    {
        return string.IsNullOrEmpty(resultName)
            || resultName == "Slop"
            || resultName == "JSON Error";
    }
}