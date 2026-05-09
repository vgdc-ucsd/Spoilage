using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class AutomaticStation : CookingStation
{
    [SerializeField] private IngredientTransform[] _transforms;

    private bool _isCooking = false;

    public override void Start()
    {
        maxIngredients = 2;
        base.Start();
    }

    public override void OnPlaceFood(FoodGrab food)
    {
        base.OnPlaceFood(food);

        if (_currentFoods.Count == 0) return;
        
        // combine ingredients
        if (_currentFoods.Count > 1)
        {   
            //stop cooking when new ingredient is added
            if (_isCooking)
            {
                _isCooking = false;
            }

            if (!TryCombineIngredients())
            {
                Debug.Log($"{gameObject.name}: Invalid combination.");
                IngredientData slop = IngredientLookup.Get("Slop");
                if (slop != null)
                {
                    IngredientObject survivor = _currentFoods[0];
                    survivor.ChangeIngredient(slop);

                    for (int i = 1; i < _currentFoods.Count; i++)
                        Destroy(_currentFoods[i].gameObject);

                    _currentFoods.Clear();
                    _currentBehaviours.Clear();
                    _currentFoods.Add(survivor);
                }
                return;
            }
        }

        if (!CanProcessCurrentIngredients())
        {
            Debug.Log($"{gameObject.name}: Nothing to cook with current ingredient.");
            return;
        }

        StartCooking();
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

    private bool CanProcessCurrentIngredients()
    {
        if (_currentFoods.Count == 1)
        {
            return TryGetTransform(_currentFoods[0].IngredientInstance.Data, out _)
                || TryGetOvercookTransform(_currentFoods[0].IngredientInstance.Data, out _);
        }

        // Multiple ingredients: check RecipeManager
        RecipeManager rm = FindAnyObjectByType<RecipeManager>();
        if (rm == null) return false;

        string result = rm.CheckRecipe(_currentFoods);
        return result != "JSON Error" && result != "Slop";
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

        IngredientData currentData = _currentFoods[0].IngredientInstance.Data;

        if (!TryGetTransform(currentData, out IngredientTransform transform))
        {
            Debug.LogWarning($"{gameObject.name}: No transform for {currentData.Name}");
            return;
        }

        _currentFoods[0].ChangeIngredient(transform.output);
        Debug.Log($"Cooking finished! {currentData.Name} → {transform.output.Name}");

        _isCooking = transform.canOvercook && transform.overcookedOutput != null;
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

    // Returns true if combination succeeded (or no combination was needed)
    private bool TryCombineIngredients()
    {
        RecipeManager rm = FindAnyObjectByType<RecipeManager>();
        if (rm == null) { Debug.LogError("RecipeManager not found!"); return false; }

        string resultName = rm.CheckRecipe(_currentFoods);
        if (resultName == "JSON Error" || resultName == "Slop")
        {
            Debug.Log($"{gameObject.name}: Invalid combination → Slop or error.");
            return false;
        }

        IngredientData resultData = IngredientLookup.Get(resultName);
        if (resultData == null) return false;

        // Collapse: change the first ingredient to the combined result, destroy the rest
        IngredientObject survivor = _currentFoods[0];
        survivor.ChangeIngredient(resultData);

        for (int i = 1; i < _currentFoods.Count; i++)
            Destroy(_currentFoods[i].gameObject);

        _currentFoods.Clear();
        _currentBehaviours.Clear();
        _currentFoods.Add(survivor);

        Debug.Log($"Combined → <b>{resultData.Name}</b>");
        return true;
    }
}