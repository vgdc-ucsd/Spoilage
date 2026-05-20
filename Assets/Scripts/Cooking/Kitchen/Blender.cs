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
            if (existing == null || existing.IngredientInstance == null) continue;

            string existingName = existing.IngredientInstance.Data.Name;
            string incomingName = incoming.IngredientInstance.Data.Name;

            if (existingName == incomingName)
            {
                Debug.Log($"{gameObject.name}: Duplicate ingredient '{incomingName}' rejected.");
                return false;
            }
        }

        bool placed = base.OnPlaceFood(food);

        LockFood();

        _currentClicks = 0;
        _isActionComplete = false;
        UpdateTimer();

        Debug.Log($"{gameObject.name}: Added ingredient. Blend clicks reset.");

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
            TurnIntoSlop();
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

        UnlockFood();

        Debug.Log($"{gameObject.name}: Blended! → {resultData.Name}");
    }

    private void TurnIntoSlop()
    {
        IngredientData slop = IngredientLookup.Get("Slop");

        if (slop == null)
        {
            Debug.LogError($"{gameObject.name}: Slop IngredientData not found.");
            ResetTimer();
            return;
        }

        IngredientObject survivor = _currentFoods[0];
        survivor.ChangeIngredient(slop);

        DestroyExtraIngredients();

        _currentFoods.Clear();
        _currentBehaviours.Clear();
        _currentFoods.Add(survivor);

        UnlockFood();
        ResetTimer();
        SetSpriteActive(true);

        Debug.Log($"{gameObject.name}: Invalid blend, turned into Slop.");
    }

    private void DestroyExtraIngredients()
    {
        for (int i = 1; i < _currentFoods.Count; i++)
        {
            if (_currentFoods[i] != null)
                Destroy(_currentFoods[i].gameObject);
        }
    }

    private void LockFood()
    {
        foreach (IngredientObject food in _currentFoods)
        {
            if (food == null) continue;

            FoodGrab grab = food.GetComponent<FoodGrab>();
            if (grab != null)
                grab.IsLocked = true;
        }
    }

    private void UnlockFood()
    {
        foreach (IngredientObject food in _currentFoods)
        {
            if (food == null) continue;

            FoodGrab grab = food.GetComponent<FoodGrab>();
            if (grab != null)
                grab.IsLocked = false;
        }
    }

    private bool IsInvalidRecipeResult(string resultName)
    {
        return string.IsNullOrEmpty(resultName)
            || resultName == "Slop"
            || resultName == "JSON Error";
    }
}