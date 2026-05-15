using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class AutomaticStation : CookingStation
{
    [Header("Cooking Settings")]
    [SerializeField] private float _cookDuration = 5f;
    [SerializeField] private float _overcookDuration = 5f;
    [SerializeField] private bool _canOvercook = false;

    [Header("Timer UI")]
    [SerializeField] private GameObject _timerObject;
    [SerializeField] private Image _timerFill;

    [SerializeField] private string _stationID;

    private float _timer;
    private bool _isCooking;
    private bool _isOverCooking = false;
    private bool _canCook = true;

    public override void Start()
    {
        maxIngredients = 3;
        base.Start();
        HideTimer();
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
            Debug.Log($"{gameObject.name}: Cannot cook Slop.");
            return false;
        }

        bool wasEmpty = _currentFoods.Count == 0;

        foreach (IngredientObject existing in _currentFoods)
        {
            if (existing != null && 
                existing.IngredientInstance.Data == incoming.IngredientInstance.Data)
            {
                Debug.Log($"{gameObject.name}: Duplicate ingredient '{incoming.IngredientInstance.Data.Name}' rejected.");
                return false;
            }
        }

        base.OnPlaceFood(food);

        //if ingredient is alr overcooked dont let place
        if (incoming.IngredientInstance.IsOvercooked)
        {
            Debug.Log($"{gameObject.name}: Rejected {incoming.name} because it is already overcooked.");
            _canCook = false;
        }
        else
        {
            _canCook = true;
        }

        if (_currentFoods.Count == 0) return false;

        food.SetLastStation(_stationID);

        if (wasEmpty)
        {
            _timer = food.GetCookTimer(_stationID);
        }
        else
        {
            // adding new ingredient resets timer
            _timer = 0f;
        }

        StartCooking();
        return true;
    }

    public override void OnRemoveFood()
    {
        if (_currentFood != null)
        {
            FoodGrab food = _currentFood.GetComponent<FoodGrab>();

            if (food != null)
            {
                food.SaveCookTimer(_stationID, _timer);
            }
        }
        if (_isCooking)
        {
            _isCooking = false;
            Debug.Log($"{gameObject.name}: Cooking interrupted.");
        }

        bool wasOvercooking = _isOverCooking;  
        StopCooking();
        _isOverCooking = wasOvercooking;
        base.OnRemoveFood();
    }

    public virtual void StartCooking()
    {
        if (!_canCook) return;
        if (_currentFoods.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name}: Tried to start cooking with no ingredients.");
            return;
        }

        _isCooking = true;

        if (_timer <= 0f)
            _isOverCooking = false;

        if (_timerFill != null)
            _timerFill.color = _isOverCooking ? Color.red : Color.green;

        SetSpriteActive(true);
        ShowTimer();

        Debug.Log($"{gameObject.name}: Started cooking {_currentFoods.Count} ingredient(s).");
    }

    private void StopCooking()
    {
        _isCooking = false;
        _isOverCooking = false;
        HideTimer();
    }

    public virtual void Update()
    {
        if (!_isCooking || _currentFoods.Count == 0) return;

        _timer += Time.deltaTime;

        float duration = _isOverCooking ? _overcookDuration : _cookDuration;

        UpdateTimer(duration);

        if (_timer < duration)
            return;

        if (_isOverCooking)
        {
            _isOverCooking = false;
            FinishOvercooking();
        }
        else
        {
            FinishCooking();
        }
    }

    public virtual void FinishCooking()
    {
        Debug.Log($"FinishCooking called. isCooking: {_isCooking}, isOvercooking: {_isOverCooking}, timer: {_timer}");
        _isCooking = false;
        if (_currentFoods.Count == 0) return;

        RecipeManager recipeManager = FindAnyObjectByType<RecipeManager>();

        if (recipeManager == null)
        {
            Debug.LogError($"{gameObject.name}: RecipeManager not found.");
            _isCooking = false;
            return;
        }

        float averageSpoilage = recipeManager.GetAverageSpoilage(_currentFoods);

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
        if (resultData == null)
        {
            Debug.LogError($"{gameObject.name}: Could not find IngredientData for result '{resultName}'.");
            StopCooking();
            return;
        }

        Recipe matchedRecipe = System.Array.Find(recipeManager.allRecipes.allRecipes, r => r.name == resultName);

        IngredientObject survivor = _currentFoods[0];
        survivor.ChangeIngredient(resultData);

        if (matchedRecipe != null && matchedRecipe.spoiled)
        {
            //stage 2 spoiled
            survivor.IngredientInstance.SetSpoilagePercent(100f);
        }
        else
        {
            survivor.IngredientInstance.SetSpoilagePercent(averageSpoilage);
        }

        DestroyExtraIngredients();

        _currentFoods.Clear();
        _currentBehaviours.Clear();
        _currentFoods.Add(survivor);

        Debug.Log($"<color=green>{gameObject.name}: SUCCESS:</color> {resultData.Name}");

        if (_canOvercook)
        {
            _timer = 0f;
            _isCooking = true;
            _isOverCooking = true;

            if (_timerFill != null) _timerFill.color = Color.red;
            Debug.Log($"Overcook started. isCooking: {_isCooking}, isOvercooking: {_isOverCooking}, timer: {_timer}, overcookDuration: {_overcookDuration}");

            ShowTimer();
            SetSpriteActive(true);
            Debug.Log($"{gameObject.name}: {resultData.Name} can continue cooking / overcook. Timer restarted.");
            return;
        }

        _timer = 0f;
        StopCooking();
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

    private void FinishOvercooking()
    {
        if (_currentFoods.Count == 0)
        {
            StopCooking();
            return;
        }

        IngredientObject food = _currentFoods[0];

        if (food != null && food.IngredientInstance != null)
        {
            food.IngredientInstance.SetOvercooked(true);
            Debug.Log($"<color=red>{gameObject.name}: {food.IngredientInstance.Data.Name} is now OVERCOOKED.</color>");
        }

        _timer = 0f;
        StopCooking();
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

        _timer = 0f;
        StopCooking();
        SetSpriteActive(true);

        Debug.Log($"{gameObject.name}: Invalid combination, turned into Slop.");
    }

    private void DestroyExtraIngredients()
    {
        for (int i = 1; i < _currentFoods.Count; i++)
        {
            if (_currentFoods[i] != null)
                Destroy(_currentFoods[i].gameObject);
        }
    }

    private bool IsInvalidRecipeResult(string resultName)
    {
        return string.IsNullOrEmpty(resultName)
            || resultName == "Slop"
            || resultName == "JSON Error";
    }

    private void UpdateTimer(float duration)
    {
        if (_timerFill == null)
        {
            return;
        }

        float progress = _timer / duration;
        _timerFill.fillAmount = Mathf.Clamp01(1f - progress);

        _timerFill.color = _isOverCooking ? Color.red : Color.green;
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
}