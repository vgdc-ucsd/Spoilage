using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class AutomaticStation : CookingStation
{

    // [SerializeField] private Timer _timer;

    // This basically tells us the station if it's a toaster, grill, etc.
    [SerializeField] private IngredientTransform[] _transforms;

    
    [Header("Cooking Settings")]
    [SerializeField] private float _cookDuration = 5f;
    [SerializeField] private float _overcookDuration = 5f;

    [Header("Timer UI")]
    [SerializeField] private GameObject _timerObject;
    [SerializeField] private Image _timerFill;


    private float _timer;
    private bool _isCooking;
    private bool _isOvercooking;

    public override void Start()
    {
        maxIngredients = 2;
        base.Start();
        HideTimer();

        // Find the GameConsole object in the scene and get its Timer component
        // _timer = GameObject.Find("GameConsole").GetComponent<Timer>();
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
        // execute any special logic after calling base to ensure the food is set before accessing it

        // SAFETY CHECK: Ensure food and timer exist
        if (_currentFood == null || _currentFood.IngredientInstance == null || _currentFood.IngredientInstance.Data == null)
        {
            Debug.LogWarning("Missing IngredientObject or IngredientBehaviour reference!");
            return;
        }

        // SAFETY CHECK: Ensure the ingredient data was assigned in the Inspector
        if (_currentFood.IngredientInstance == null || _currentFood.IngredientInstance.Data == null)
        {
            Debug.LogError("Food data is missing on " + _currentFood.name);
            return;
        }

        IngredientData currentData = _currentFood.IngredientInstance.Data;

        if (!TryGetTransform(currentData, out IngredientTransform transform) &&
            !TryGetOvercookTransform(currentData, out transform))
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

        StopCooking();
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

    public void StartCooking()
    {
        _timer = 0f;
        _isCooking = true;
        Debug.Log($"{gameObject.name}: Started cooking {_currentFoods.Count} ingredient(s).");
    

        _isOvercooking = false;

        ShowTimer();
        Debug.Log("Started cooking");
    }

    private void StopCooking()
    {
        _timer = 0f;
        _isCooking = false;
        _isOvercooking = false;
        HideTimer();
    }

    public virtual void Update()
    {
        if (!_isCooking || _currentFoods.Count == 0) return;

        _timer += Time.deltaTime;

        float duration = _isOvercooking ? _overcookDuration : _cookDuration;
        UpdateTimer(duration);
    
        if (_timer < duration)
        {
            return;
        }

        if (_isOvercooking)
        {
            FinishOvercooking();
            return;
        }

        FinishCooking();
    }

    private void UpdateTimer(float duration)
    {
        if (_timerFill == null)
        {
            return;
        }

        float progress = _timer / duration;
        _timerFill.fillAmount = Mathf.Clamp01(1f - progress);

        if (_isOvercooking)
        {
            _timerFill.color = Color.red;
        }
    }

    private void ShowTimer()
    {
        if (_timerObject != null)
        {
            _timerObject.SetActive(true);
        }
    }

    private void HideTimer()
    {
        if (_timerObject != null)
        {
            _timerObject.SetActive(false);
        }
    }


    public virtual void FinishCooking()
    {
        _isCooking = false;
        if (_currentFoods.Count == 0) return;

        IngredientData currentData = _currentFoods[0].IngredientInstance.Data;
        if (_currentFood == null || _currentFood.IngredientInstance == null)
        {
            StopCooking();
            return;
        }

        if (!TryGetTransform(currentData, out IngredientTransform transform))
        {
            Debug.LogWarning($"{gameObject.name} has no transform for {currentData.Name}");
            StopCooking();
            return;
        }

        _currentFoods[0].ChangeIngredient(transform.output);
        Debug.Log($"Cooking finished! {currentData.Name} → {transform.output.Name}");
        if (transform.output == null)
        {
            Debug.LogError("Output ingredient is missing on " + gameObject.name);
            StopCooking();
            return;
        }


        _isCooking = transform.canOvercook && transform.overcookedOutput != null;
        if (transform.canOvercook && transform.overcookedOutput != null)
        {
            _timer = 0f;
            _isCooking = true;
            _isOvercooking = true;
            return;
        }
        StopCooking();
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

        if (_currentFood == null || _currentFood.IngredientInstance == null)
        {
            StopCooking();
            return;
        }


        if (!transform.canOvercook)
        {
            return;
        }

        if (transform.overcookedOutput == null)
        {
            Debug.LogWarning("Overcooked ingredient is missing on " + gameObject.name);
            return;
        }

        _currentFood.ChangeIngredient(transform.overcookedOutput);
        Debug.Log($"Food overcooked! {currentData.Name} is now {transform.overcookedOutput.Name}!");
        
        StopCooking();
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
