using UnityEngine;
using System.Collections.Generic;

public class CuttingBoard : ManualStation
{
    // --- Cutting Board Speed: stacking -10% clicks per repeated recipe ---
    private string _lastCutRecipe = "";
    private int _speedStack = 0;
    private const float SPEED_STACK_REDUCTION = 0.10f;

    // --- Auto Cutting Board: automated timer replaces manual clicking ---
    private bool _isAutoProcessing = false;
    private float _autoTimer = 0f;
    [SerializeField] private float _autoCutDuration = 3f;

    public override void Start()
    {
        maxIngredients = 1;
        base.Start();
    }

    public override bool OnPlaceFood(FoodGrab food)
    {
        IngredientObject incoming = food.GetComponent<IngredientObject>();
        if (incoming == null) return false;

        if (_currentFoods.Contains(incoming))
            return true;

        if (incoming.IngredientInstance.Data.Name == RecipeManager.SlopResult)
            return false;

        if (!HasSpace)
        {
            Debug.LogWarning($"{gameObject.name}: Cutting board only accepts one ingredient.");
            return false;
        }

        bool placed = base.OnPlaceFood(food);

        if (_currentFood == null || _currentFood.IngredientInstance == null)
            return false;

        RecipeManager recipeManager = FindAnyObjectByType<RecipeManager>();
        if (recipeManager == null)
        {
            Debug.LogError($"{gameObject.name}: RecipeManager not found.");
            return false;
        }

        List<IngredientObject> check = new() { _currentFood };
        string result = recipeManager.CheckRecipe(check, _station);

        if (!RecipeManager.IsSuccessfulRecipeResult(result))
        {
            Debug.Log($"{gameObject.name}: Wrong ingredient for cutting board.");
            HideManualUI();
            return true;
        }

        // Auto Cutting Board: start timer and hide manual UI
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.HasUpgrade("auto_cutting_board"))
        {
            _isAutoProcessing = true;
            _autoTimer = 0f;
            HideManualUI();
        }

        Debug.Log($"{gameObject.name}: Food on cutting board.");
        return placed;
    }

    public override void OnRemoveFood()
    {
        _isAutoProcessing = false;
        _autoTimer = 0f;
        base.OnRemoveFood();
    }

    private void Update()
    {
        if (!_isAutoProcessing || _currentFood == null) return;

        _autoTimer += Time.deltaTime;
        float effectiveDuration = _autoCutDuration * GetAutoSpeedMultiplier();

        if (_autoTimer >= effectiveDuration)
        {
            _isAutoProcessing = false;
            CompleteManualAction();
            ApplyQualityBonus(_currentFood);
        }
    }

    public void PressCutButton()
    {
        Debug.Log($"{gameObject.name}: Cut button pressed.");
        OnAction();
    }

    public override void OnAction()
    {
        if (_isAutoProcessing) return; // auto mode; ignore manual clicks
        base.OnAction();
    }

    protected override int GetEffectiveClicksPerState()
    {
        if (UpgradeManager.Instance == null || !UpgradeManager.Instance.HasUpgrade("cutting_board_speed"))
            return _clicksPerState;
        return Mathf.Max(1, Mathf.RoundToInt(_clicksPerState * GetAutoSpeedMultiplier()));
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

        List<IngredientObject> ingredients = new() { _currentFood };
        string resultName = recipeManager.CheckRecipe(ingredients, _station);

        IngredientData resultData = IngredientLookup.Get(resultName);
        if (resultData == null)
        {
            Debug.LogError($"{gameObject.name}: Could not find IngredientData for result '{resultName}'.");
            ResetTimer();
            return;
        }

        // Cutting Board Speed: advance stack before changing ingredient
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.HasUpgrade("cutting_board_speed"))
        {
            if (resultName == _lastCutRecipe) _speedStack++;
            else _speedStack = 0;
            _lastCutRecipe = resultName;
        }

        bool usedUnspoiledFood = SpoilageTriggerManager.IsUnspoiledFood(_currentFood);

        _currentFood.ChangeIngredient(resultData);

        // Cutting Board Quality: add up to +20 quality based on ingredient freshness
        if (UpgradeManager.Instance != null
            && UpgradeManager.Instance.HasUpgrade("cutting_board_quality")
            && _currentFood.IngredientInstance != null)
        {
            float freshnessFraction = 1f - (_currentFood.IngredientInstance.SpoilagePercent / 100f);
            float qualityBonus = freshnessFraction * 20f;
            _currentFood.QualityPercent = Mathf.Clamp(_currentFood.QualityPercent + qualityBonus, 0f, 100f);
        }

        SpoilageTriggerManager.TriggerIf(SpoilageCategory.DISGUST, usedUnspoiledFood);

        Debug.Log($"{gameObject.name}: Chopped! → {resultData.Name}");
    }

    /// <summary>Speed multiplier from Cutting Board Speed stacks; also used to reduce auto-cut duration.</summary>
    private float GetAutoSpeedMultiplier()
        => Mathf.Max(1f - (_speedStack * SPEED_STACK_REDUCTION), 0.10f);
}
