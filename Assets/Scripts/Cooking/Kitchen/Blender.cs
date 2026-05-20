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

    if (incoming == null)
        return false;

    // reject slop
    if (incoming.IngredientInstance.Data.Name == "Slop")
        return false;

    // reject duplicates FIRST
    foreach (IngredientObject existing in _currentFoods)
    {
        if (existing == null || existing.IngredientInstance == null)
            continue;

        string existingName = existing.IngredientInstance.Data.Name;
        string incomingName = incoming.IngredientInstance.Data.Name;

        Debug.Log($"Comparing existing '{existingName}' with incoming '{incomingName}'");

        if (existingName.Trim().ToLower() ==
            incomingName.Trim().ToLower())
        {
            Debug.Log($"{gameObject.name}: Duplicate ingredient rejected.");

            FoodGrab incomingGrab = incoming.GetComponent<FoodGrab>();

            if (incomingGrab != null)
                incomingGrab.IsLocked = false;

            return false;
        }
    }

    // capacity check
    if (!HasSpace)
    {
        Debug.LogWarning($"{gameObject.name}: Blender full.");
        return false;
    }

    Debug.Log($"BEFORE base.OnPlaceFood: {_currentFoods.Count}");
    bool placed = base.OnPlaceFood(food);
    Debug.Log($"AFTER base.OnPlaceFood: {_currentFoods.Count}");

    if (!placed)
        return false;

    LockFood();

    _currentClicks = 0;
    _isActionComplete = false;

    UpdateTimer();

    Debug.Log($"{gameObject.name}: Added ingredient. Blend clicks reset.");

    return true;
}

    public void PressBlendButton()
    {
        Debug.Log($"{gameObject.name}: Blend button pressed.");
        OnAction();
    }

    protected override void CompleteManualAction()
    {
        Debug.Log($"Current foods count: {_currentFoods.Count}");

        RecipeManager recipeManager = FindAnyObjectByType<RecipeManager>();

        if (recipeManager == null)
        {
            Debug.LogError($"{gameObject.name}: RecipeManager not found.");
            ResetTimer();
            return;
        }

        string resultName = recipeManager.CheckRecipe(_currentFoods, _station);

        // invalid recipe = slop
        if (IsInvalidRecipeResult(resultName))
        {
            TurnIntoSlop();
            return;
        }

        IngredientData resultData = IngredientLookup.Get(resultName);

        if (resultData == null)
        {
            Debug.LogError($"{gameObject.name}: Could not find IngredientData for '{resultName}'.");
            ResetTimer();
            return;
        }

        IngredientObject survivor = _currentFoods[0];

        survivor.ChangeIngredient(resultData);

        DestroyExtraIngredients(survivor);

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

        DestroyExtraIngredients(survivor);

        _currentFoods.Clear();
        _currentBehaviours.Clear();
        _currentFoods.Add(survivor);

        UnlockFood();

        ResetTimer();

        SetSpriteActive(true);

        Debug.Log($"{gameObject.name}: Invalid blend, turned into Slop.");
    }

    private void DestroyExtraIngredients(IngredientObject survivor)
    {
        foreach (IngredientObject food in _currentFoods)
        {
            if (food != null && food != survivor)
            {
                Destroy(food.gameObject);
            }
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