using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class AutomaticStation : CookingStation
{
    [Header("Cooking Settings")]
    [SerializeField] private const int OVERCOOKED_QUALITY_PERCENTAGE_DECREASE = 20;
    [SerializeField] private float _cookDuration = 5f;
    [SerializeField] private float _overcookDuration = 5f;
    [SerializeField] private bool _canOvercook = false;

    [Header("Timer UI")]
    [SerializeField] private GameObject _timerObject;
    [SerializeField] private Image _timerFill;

    [SerializeField] private string _stationID;

    // Colors for timer fill: normal (green) and overcooking (red)
    private static readonly Color32 s_normalColor = new Color32(83, 242, 117, 255);
    private static readonly Color32 s_overcookColor = new Color32(255, 0, 0, 255);

    private float _timer;
    public bool _isCooking;
    public bool _isOverCooking = false;
    private bool _canCook = true;

    // --- Grill Quality: stacking +5% per same recipe, reset on change ---
    private string _lastRecipeName = "";
    private int _grillQualityStack = 0;
    private const float GRILL_QUALITY_PER_STACK = 5f;

    // --- Hotter Grill: 25% speed boost when next recipe starts within 10 s ---
    private float _timeSinceLastFinish = float.MaxValue;
    private bool _hotterGrillBoostActive = false;
    private const float HOTTER_GRILL_WINDOW = 10f;
    private const float HOTTER_GRILL_BOOST = 0.25f;

    // --- Pot Quality: quality bonus based on recipe variety (last 4 recipes) ---
    private readonly Queue<string> _recentRecipes = new();
    private const int POT_QUALITY_HISTORY = 4;
    private const float POT_QUALITY_MAX_BONUS = 20f;

    // --- Preheat (Oven): passive preheat timer gives 25% speed boost for one recipe ---
    private float _preheatProgress = 0f;
    private bool _preheatReady = false;
    private bool _preheatBoostThisRecipe = false; // latched when preheat is consumed; checked by GetEffectiveCookDuration
    private const float PREHEAT_DURATION = 30f;
    private const float PREHEAT_SPEED_BOOST = 0.25f;
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
        
        if (incoming.IngredientInstance.Data.Name == RecipeManager.SlopResult)
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
                Destroy(incoming.gameObject);
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

        // Hotter Grill: activate speed boost if within the restart window
        _hotterGrillBoostActive = UpgradeManager.Instance != null
            && UpgradeManager.Instance.HasUpgrade("hotter_grill")
            && _station == "Grill"
            && _timeSinceLastFinish <= HOTTER_GRILL_WINDOW;

        // Preheat: latch boost flag BEFORE clearing _preheatReady so GetEffectiveCookDuration sees it
        _preheatBoostThisRecipe = _preheatReady
            && UpgradeManager.Instance != null
            && UpgradeManager.Instance.HasUpgrade("preheat")
            && _station == "Oven";
        if (_preheatBoostThisRecipe)
        {
            _preheatReady = false;
            _preheatProgress = 0f;
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

        // Oven Quality: pulling food out early rewards the player; the quality bonus
        // follows a bell curve — peaks (+15) around the halfway point of the overcook
        // window and returns to 0 at both ends, so full-completion still penalises.
        if (wasOvercooking
            && _currentFood != null
            && UpgradeManager.Instance != null
            && UpgradeManager.Instance.HasUpgrade("oven_quality")
            && _station == "Oven")
        {
            float progress = Mathf.Clamp01(_overcookDuration > 0f ? _timer / _overcookDuration : 0f);
            float bonus = 15f * (1f - Mathf.Abs(progress * 2f - 1f));
            _currentFood.QualityPercent = Mathf.Clamp(_currentFood.QualityPercent + bonus, 0f, 100f);
            Debug.Log($"{gameObject.name}: Oven Quality early removal — overcook progress {progress:P0}, bonus +{bonus:F1}. Quality = {_currentFood.QualityPercent}");
        }

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

        if (IsTemperatureStation())
        {
            SpoilageTriggerManager.Trigger(SpoilageCategory.TEMPERATURE);
        }

        if (_isOverCooking)
            UnlockFood(); // stay unlocked during overcook
        else
            LockFood();  // lock during normal cooking

        if (_timer <= 0f)
            _isOverCooking = false;

        if (_timerFill != null)
            _timerFill.color = _isOverCooking ? s_overcookColor : s_normalColor;

        SetSpriteActive(true);
        ShowTimer();

        Debug.Log($"{gameObject.name}: Started cooking {_currentFoods.Count} ingredient(s).");

        // Instant Cooking: set timer to completion so Update() finishes it next tick
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.TryUseInstantCooking())
        {
            _timer = GetEffectiveCookDuration();
            Debug.Log($"{gameObject.name}: Instant cook used!");
        }
    }

    // Returns cook duration after applying Hotter Grill and Preheat boosts
    private float GetEffectiveCookDuration()
    {
        float duration = _cookDuration;
        if (_hotterGrillBoostActive)
            duration *= (1f - HOTTER_GRILL_BOOST);
        if (_preheatBoostThisRecipe)
            duration *= (1f - PREHEAT_SPEED_BOOST);
        return Mathf.Max(duration, 0.1f);
    }

    private void StopCooking()
    {
        _isCooking = false;
        _isOverCooking = false;
        _preheatBoostThisRecipe = false;
        UnlockFood();
        HideTimer();
    }

    public virtual void Update()
    {
        // Idle timers (run even when not cooking)
        if (!_isCooking)
        {
            // Hotter Grill: track time since last recipe completion
            if (_timeSinceLastFinish < float.MaxValue)
                _timeSinceLastFinish += Time.deltaTime;

            // Preheat: passively build up when oven is idle
            if (!_preheatReady
                && UpgradeManager.Instance != null
                && UpgradeManager.Instance.HasUpgrade("preheat")
                && _station == "Oven")
            {
                _preheatProgress += Time.deltaTime / PREHEAT_DURATION;
                if (_preheatProgress >= 1f)
                {
                    _preheatProgress = 1f;
                    _preheatReady = true;
                    Debug.Log($"{gameObject.name}: Oven fully preheated.");
                }
            }
        }

        if (!_isCooking || _currentFoods.Count == 0) return;

        _timer += Time.deltaTime;

        float duration = _isOverCooking ? _overcookDuration : GetEffectiveCookDuration();

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
        bool usedUnspoiledFood = SpoilageTriggerManager.ContainsUnspoiledFood(_currentFoods);

        foreach (IngredientObject food in _currentFoods)
        {
            Debug.Log($"{gameObject.name}: On station: '{food.IngredientInstance.Data.Name}'");
        }

        string resultName = recipeManager.CheckRecipe(_currentFoods, _station);

        if (!RecipeManager.IsSuccessfulRecipeResult(resultName))
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
        survivor.QualityPercent = recipeManager.CalculateTotalQuality(_currentFoods);

        // --- Station upgrade quality applications ---
        if (UpgradeManager.Instance != null)
        {
            // Grill Quality: stacking +5% per same recipe repeat
            if (UpgradeManager.Instance.HasUpgrade("grill_quality") && _station == "Grill")
            {
                if (resultName == _lastRecipeName) _grillQualityStack++;
                else _grillQualityStack = 0;
                survivor.QualityPercent += _grillQualityStack * GRILL_QUALITY_PER_STACK;
            }

            // Pot Quality: bonus based on variety in last 4 recipes
            if (UpgradeManager.Instance.HasUpgrade("pot_quality") && _station == "Pot")
            {
                int uniqueCount = new System.Collections.Generic.HashSet<string>(_recentRecipes).Count;
                float varietyFraction = _recentRecipes.Count > 0
                    ? (float)uniqueCount / POT_QUALITY_HISTORY
                    : 0f;
                survivor.QualityPercent += varietyFraction * POT_QUALITY_MAX_BONUS;
            }

            // Pot Quality 2: input quality boosts output by an extra 50%
            if (UpgradeManager.Instance.HasUpgrade("pot_quality_2") && _station == "Pot")
            {
                float avgInputQuality = recipeManager.CalculateTotalQuality(_currentFoods);
                survivor.QualityPercent += avgInputQuality * 0.50f;
            }
        }

        _lastRecipeName = resultName;
        if (_recentRecipes.Count >= POT_QUALITY_HISTORY) _recentRecipes.Dequeue();
        _recentRecipes.Enqueue(resultName);
        _timeSinceLastFinish = 0f;

        ApplyQualityBonus(survivor);
        survivor.QualityPercent = Mathf.Clamp(survivor.QualityPercent, 0f, 100f);

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

        Debug.Log($"<color=green>{gameObject.name}: SUCCESS:</color> {resultData.Name}. Quality = {survivor.QualityPercent}");

        SpoilageTriggerManager.TriggerIf(SpoilageCategory.DISGUST, usedUnspoiledFood);

        if (_canOvercook)
        {
            _timer = 0f;
            _isCooking = true;
            _isOverCooking = true;
            UnlockFood();

            if (_timerFill != null) _timerFill.color = s_overcookColor;
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

        if (!RecipeManager.IsSuccessfulRecipeResult(nextResult))
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
            // Overcook fully completed — always apply the penalty regardless of upgrades.
            // Oven Quality only rewards *early* removal (handled in OnRemoveFood).
            food.IngredientInstance.SetOvercooked(true);
            IngredientBehaviour behaviour = food.GetComponent<IngredientBehaviour>();
            if (behaviour != null)
                behaviour.SetBurntOverlay(true);
            Debug.Log($"<color=red>{gameObject.name}: {food.IngredientInstance.Data.Name} is now OVERCOOKED.</color> Quality = {food.QualityPercent}");
        }

        _timer = 0f;
        StopCooking();
    }

    private void TurnIntoSlop()
    {
        IngredientData slop = IngredientLookup.Get(RecipeManager.SlopResult);

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

        SpoilageTriggerManager.Trigger(SpoilageCategory.HUNGER);

        Debug.Log($"{gameObject.name}: Invalid combination, turned into Slop.");
    }

    private bool IsTemperatureStation()
    {
        return string.Equals(_station, "Grill", System.StringComparison.OrdinalIgnoreCase)
            || string.Equals(_station, "Pot", System.StringComparison.OrdinalIgnoreCase);
    }

    private void DestroyExtraIngredients()
    {
        for (int i = 1; i < _currentFoods.Count; i++)
        {
            if (_currentFoods[i] != null)
                Destroy(_currentFoods[i].gameObject);
        }
    }

    private void UpdateTimer(float duration)
    {
        if (_timerFill == null)
        {
            return;
        }

        float progress = _timer / duration;
        _timerFill.fillAmount = Mathf.Clamp01(1f - progress);

        _timerFill.color = _isOverCooking ? s_overcookColor : s_normalColor;
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

    private void LockFood()
    {
        foreach (IngredientObject food in _currentFoods)
        {
            if (food == null) continue;
            FoodGrab grab = food.GetComponent<FoodGrab>();
            if (grab != null) grab.IsLocked = true;
        }
    }

    private void UnlockFood()
    {
        foreach (IngredientObject food in _currentFoods)
        {
            if (food == null) continue;
            FoodGrab grab = food.GetComponent<FoodGrab>();
            if (grab != null) grab.IsLocked = false;
        }
    }

    public override void ApplyUpgrade(UpgradeData upgrade)
    {
        if (upgrade.upgradeType == UpgradeType.SpeedBoost)
        {
            float reduction = Mathf.Clamp01(upgrade.value);
            _cookDuration *= (1f - reduction);
            Debug.Log($"{gameObject.name}: Cook duration reduced to {_cookDuration:F2}s.");
            return;
        }

        base.ApplyUpgrade(upgrade);
    }
}
