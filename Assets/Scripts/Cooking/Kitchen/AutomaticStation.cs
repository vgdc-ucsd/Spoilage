using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class AutomaticStation : CookingStation
{
    [SerializeField] private IngredientTransform[] _transforms;

    private bool _isCooking = false;

    public override void Start()
    {
        maxIngredients = 3;
        base.Start();
    }

    public override void OnPlaceFood(FoodGrab food)
    {
        base.OnPlaceFood(food);

        if (_currentFoods.Count == 0) return;
        
        StartCooking();
        Debug.Log($"Ingredient added to {gameObject.name}");
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
        _isCooking = true;
        Debug.Log($"{gameObject.name}: Started cooking {_currentFoods.Count} ingredient(s).");
    }

    public virtual void Update()
    {
        if (!_isCooking || _currentFoods.Count == 0) return;

        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            Debug.Log("T key pressed — finishing cooking.");
            FinishCooking();
        }
    }

    public virtual void FinishCooking()
    {
        _isCooking = false;
        if (_currentFoods.Count == 0) return;
        
        foreach (var food in _currentFoods)
        Debug.Log($"On station: '{food.IngredientInstance.Data.Name}'");

        RecipeManager rm = FindAnyObjectByType<RecipeManager>();
        if (rm == null) { Debug.LogError("RecipeManager not found!"); return; }

        string resultName = rm.CheckRecipe(_currentFoods, _station);
        IngredientData resultData = resultName != "Slop" && resultName != "JSON Error"
            ? IngredientLookup.Get(resultName)
            : null;

        if (resultData != null)
        {
            IngredientObject survivor = _currentFoods[0];
            survivor.ChangeIngredient(resultData);

            for (int i = 1; i < _currentFoods.Count; i++)
                Destroy(_currentFoods[i].gameObject);

            _currentFoods.Clear();
            _currentBehaviours.Clear();
            _currentFoods.Add(survivor);

            if (TryGetTransform(resultData, out IngredientTransform overcookTransform))
            {
                _isCooking = true;
                Debug.Log($"Can overcook into {overcookTransform.output.Name}, timer restarted.");
                return;
            }

            Debug.Log($"<color=green>SUCCESS:</color> {resultData.Name}");
        }
        else
        {
            TurnIntoSlop();
            return;
        }
        _currentFoods.Clear();
        _currentBehaviours.Clear();
        SetSpriteActive(false);
    }

    public virtual void FinishOvercooking()
    {
        _isCooking = false;
        if (_currentFoods.Count == 0) return;

        IngredientData currentData = _currentFoods[0].IngredientInstance.Data;
        if (!TryGetOvercookTransform(currentData, out IngredientTransform transform)) return;
        if (!transform.canOvercook || transform.overcookedOutput == null) return;

        _currentFoods[0].ChangeIngredient(transform.overcookedOutput);
        Debug.Log($"Overcooked! {currentData.Name} → {transform.overcookedOutput.Name}");
    }

    private bool TryGetTransform(IngredientData input, out IngredientTransform match)
    {
        foreach (var t in _transforms)
            if (t.input == input) { match = t; return true; }
        match = null; return false;
    }

    private bool TryGetOvercookTransform(IngredientData input, out IngredientTransform match)
    {
        foreach (var t in _transforms)
            if (t.output == input) { match = t; return true; }
        match = null; return false;
    }

    private void TurnIntoSlop()
    {
        IngredientData slop = IngredientLookup.Get("Slop");
        if (slop == null) return;

        IngredientObject survivor = _currentFoods[0];
        survivor.ChangeIngredient(slop);

        for (int i = 1; i < _currentFoods.Count; i++)
            Destroy(_currentFoods[i].gameObject);

        _currentFoods.Clear();
        _currentBehaviours.Clear();
        _currentFoods.Add(survivor);
    
        SetSpriteActive(true);
        Debug.Log($"Invalid combination, turned into slop.");
    }
}