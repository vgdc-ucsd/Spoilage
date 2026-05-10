using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class AutomaticStation : CookingStation
{
    private bool _isCooking = false;

    public override void Start()
    {
        maxIngredients = 3;
        base.Start();
    }

    public override void OnPlaceFood(FoodGrab food)
    {
        if (!HasSpace)
        {
            Debug.LogWarning($"{gameObject.name}: Cannot place more food. Station is full.");
            return;
        }

        base.OnPlaceFood(food);

        if (_currentFoods.Count == 0)
        {
            return;
        }

        StartCooking();
        Debug.Log($"{gameObject.name}: Ingredient added.");
    }

    public override void OnRemoveFood()
    {
        if (_isCooking)
        {
            _isCooking = false;
            Debug.Log($"{gameObject.name}: Cooking interrupted.");
        }

        base.OnRemoveFood();
    }

    public virtual void StartCooking()
    {
        if (_currentFoods.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name}: Tried to start cooking with no ingredients.");
            return;
        }

        _isCooking = true;
        SetSpriteActive(true);

        Debug.Log($"{gameObject.name}: Started cooking {_currentFoods.Count} ingredient(s).");
    }

    public virtual void Update()
    {
        if (!_isCooking || _currentFoods.Count == 0)
        {
            return;
        }

        // Temporary test input until real timer is implemented
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            Debug.Log($"{gameObject.name}: T key pressed — finishing cooking.");
            FinishCooking();
        }
    }

    public virtual void FinishCooking()
    {
        if (_currentFoods.Count == 0)
        {
            _isCooking = false;
            return;
        }

        RecipeManager recipeManager = FindAnyObjectByType<RecipeManager>();

        if (recipeManager == null)
        {
            Debug.LogError($"{gameObject.name}: RecipeManager not found.");
            _isCooking = false;
            return;
        }

        foreach (IngredientObject food in _currentFoods)
        {
            Debug.Log($"{gameObject.name}: On station: '{food.IngredientInstance.Data.Name}'");
        }

        string resultName = recipeManager.CheckRecipe(_currentFoods, _station);

        if (IsInvalidRecipeResult(resultName))
        {
            TurnIntoSlop();
            return;
        }

        IngredientData resultData = IngredientLookup.Get(resultName);

        Debug.Log($"Recipe result from JSON: '{resultName}'");
        if (resultData == null)
        {
            Debug.LogError($"{gameObject.name}: Could not find IngredientData for result '{resultName}'.");
            TurnIntoSlop();
            return;
        }

        IngredientObject survivor = _currentFoods[0];
        survivor.ChangeIngredient(resultData);

        DestroyExtraIngredients();

        _currentFoods.Clear();
        _currentBehaviours.Clear();
        _currentFoods.Add(survivor);

        Debug.Log($"<color=green>{gameObject.name}: SUCCESS:</color> {resultData.Name}");

        if (CanContinueCooking(recipeManager, survivor))
        {
            _isCooking = true;
            SetSpriteActive(true);
            Debug.Log($"{gameObject.name}: {resultData.Name} can continue cooking / overcook. Timer restarted.");
            return;
        }

        _isCooking = false;
        ClearStationTracking();
    }

    private bool CanContinueCooking(RecipeManager recipeManager, IngredientObject food)
    {
        if (food == null || food.IngredientInstance == null)
        {
            return false;
        }

        List<IngredientObject> singleIngredient = new() { food };
        string nextResult = recipeManager.CheckRecipe(singleIngredient, _station);

        if (IsInvalidRecipeResult(nextResult))
        {
            return false;
        }

        IngredientData currentData = food.IngredientInstance.Data;

        // Prevent infinite loop if recipe accidentally outputs itself
        if (nextResult == currentData.Name)
        {
            Debug.LogWarning($"{gameObject.name}: Recipe result is same as input. Preventing infinite cook loop.");
            return false;
        }

        return IngredientLookup.Get(nextResult) != null;
    }

    private void TurnIntoSlop()
    {
        IngredientData slop = IngredientLookup.Get("Slop");

        if (slop == null)
        {
            Debug.LogError($"{gameObject.name}: Slop IngredientData not found.");
            _isCooking = false;
            ClearStationTracking();
            return;
        }

        IngredientObject survivor = _currentFoods[0];
        survivor.ChangeIngredient(slop);

        DestroyExtraIngredients();

        _currentFoods.Clear();
        _currentBehaviours.Clear();
        _currentFoods.Add(survivor);

        _isCooking = false;
        SetSpriteActive(true);

        Debug.Log($"{gameObject.name}: Invalid combination, turned into Slop.");
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