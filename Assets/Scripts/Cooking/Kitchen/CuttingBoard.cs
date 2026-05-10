using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CuttingBoard : ManualStation
{

    // TODO: delete this once popup button is implemented
    void Update()
    {
        if (_currentFood == null) return;

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame) // F key
        {
            OnAction();
        }
    }

    public override void OnPlaceFood(FoodGrab food)
    {
        base.OnPlaceFood(food);

        if (_currentFood == null || _currentFood.IngredientInstance == null) return;

        Debug.Log("Food on cutting board");

        RecipeManager rm = FindAnyObjectByType<RecipeManager>();
        if (rm == null) return;

        // Warn early if nothing can be made with this ingredient here
        List<IngredientObject> check = new List<IngredientObject> { _currentFood };
        string result = rm.CheckRecipe(check, _station);
        if (result == "Slop" || result == "JSON Error")
            Debug.Log("Wrong ingredient for cutting board");
    }

    public override void OnAction()
    {
        Debug.Log($"Action triggered on {gameObject.name}. Current Food: {(_currentFood != null ? _currentFood.name : "NULL")}");
        if (_currentFood == null || _currentFood.IngredientInstance == null) return;

        base.OnAction();

        if (_currentClicks < _clicksPerState) return;

        RecipeManager rm = FindAnyObjectByType<RecipeManager>();
        if (rm == null) { Debug.LogError("RecipeManager not found!"); return; }

        List<IngredientObject> ingredients = new List<IngredientObject> { _currentFood };
        string resultName = rm.CheckRecipe(ingredients, _station);

        if (resultName == "Slop" || resultName == "JSON Error")
        {
            Debug.Log("Wrong ingredient for cutting board");
            _currentClicks = 0;
            return;
        }

        IngredientData resultData = IngredientLookup.Get(resultName);
        if (resultData == null) return;

        _currentFood.ChangeIngredient(resultData);
        Debug.Log($"Chopped! → {resultData.Name}");
        _currentClicks = 0;
    }
}