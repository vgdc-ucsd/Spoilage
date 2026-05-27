using UnityEngine;
using System.Collections.Generic;

public class Blender : ManualStation
{
    // --- Auto Blender: automated blend timer ---
    private bool _isAutoBlending = false;
    private float _autoBlendTimer = 0f;
    [SerializeField] private float _autoBlendDuration = 4f;

    // --- Blender Quality: post-blend rest period for quality gain ---
    private bool _isPostBlending = false;
    private float _postBlendTimer = 0f;
    private const float POST_BLEND_DURATION = 5f;
    private const float POST_BLEND_MAX_BONUS = 20f;

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

        if (incoming.IngredientInstance.Data.Name == RecipeManager.SlopResult)
            return false;

        foreach (IngredientObject existing in _currentFoods)
        {
            if (existing == null || existing.IngredientInstance == null)
                continue;

            string existingName = existing.IngredientInstance.Data.Name;
            string incomingName = incoming.IngredientInstance.Data.Name;

            Debug.Log($"Comparing existing '{existingName}' with incoming '{incomingName}'");

            if (existingName.Trim().ToLower() == incomingName.Trim().ToLower())
            {
                Debug.Log($"{gameObject.name}: Duplicate ingredient rejected.");
                Destroy(incoming.gameObject);
                return false;
            }
        }

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

        // Auto Blender: reset timer whenever a new ingredient is added
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.HasUpgrade("auto_blender"))
        {
            _isAutoBlending = true;
            _autoBlendTimer = 0f;
            HideManualUI();
        }
        else
        {
            _isAutoBlending = false;
        }

        _currentClicks = 0;
        _isActionComplete = false;
        _isPostBlending = false;

        UpdateTimer();

        Debug.Log($"{gameObject.name}: Added ingredient. Blend clicks reset.");
        return true;
    }

    public override void OnRemoveFood()
    {
        // Blender Quality: award partial quality bonus if removed during post-blend rest
        if (_isPostBlending && _currentFood != null && _postBlendTimer > 0f)
        {
            float fraction = Mathf.Clamp01(_postBlendTimer / POST_BLEND_DURATION);
            float bonus = fraction * POST_BLEND_MAX_BONUS;
            _currentFood.QualityPercent = Mathf.Clamp(_currentFood.QualityPercent + bonus, 0f, 100f);
            Debug.Log($"{gameObject.name}: Blender Quality bonus applied ({bonus:F1}) on early removal.");
        }

        _isAutoBlending = false;
        _autoBlendTimer = 0f;
        _isPostBlending = false;
        _postBlendTimer = 0f;
        base.OnRemoveFood();
    }

    private void Update()
    {
        // Auto Blender: fire CompleteManualAction when timer expires
        if (_isAutoBlending && _currentFoods.Count > 0)
        {
            _autoBlendTimer += Time.deltaTime;
            if (_autoBlendTimer >= _autoBlendDuration)
            {
                _isAutoBlending = false;
                CompleteManualAction();
                ApplyQualityBonus(_currentFood);
            }
            return;
        }

        // Blender Quality: accumulate quality during post-blend rest
        if (_isPostBlending && _currentFood != null)
        {
            _postBlendTimer += Time.deltaTime;
            if (_timerFill != null)
                _timerFill.fillAmount = Mathf.Clamp01(_postBlendTimer / POST_BLEND_DURATION);

            if (_postBlendTimer >= POST_BLEND_DURATION)
            {
                _isPostBlending = false;
                _currentFood.QualityPercent = Mathf.Clamp(
                    _currentFood.QualityPercent + POST_BLEND_MAX_BONUS, 0f, 100f);
                if (_timerFill != null) _timerFill.fillAmount = 1f;
                Debug.Log($"{gameObject.name}: Blender Quality fully applied. Quality = {_currentFood.QualityPercent}");
            }
        }
    }

    public void PressBlendButton()
    {
        Debug.Log($"{gameObject.name}: Blend button pressed.");
        OnAction();
    }

    public override void OnAction()
    {
        if (_isAutoBlending) return; // auto mode; ignore manual clicks
        base.OnAction();
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

        if (!RecipeManager.IsSuccessfulRecipeResult(resultName))
        {
            TurnIntoSlop();
            HideManualUI();
            return;
        }

        IngredientData resultData = IngredientLookup.Get(resultName);

        if (resultData == null)
        {
            Debug.LogError($"{gameObject.name}: Could not find IngredientData for '{resultName}'.");
            ResetTimer();
            return;
        }

        bool usedUnspoiledFood = SpoilageTriggerManager.ContainsUnspoiledFood(_currentFoods);
        IngredientObject survivor = _currentFoods[0];

        survivor.ChangeIngredient(resultData);
        DestroyExtraIngredients(survivor);

        _currentFoods.Clear();
        _currentBehaviours.Clear();
        _currentFoods.Add(survivor);

        UnlockFood();

        SpoilageTriggerManager.TriggerIf(SpoilageCategory.DISGUST, usedUnspoiledFood);

        // Blender Productivity: 33% chance to produce one extra copy
        if (UpgradeManager.Instance != null
            && UpgradeManager.Instance.HasUpgrade("blender_productivity")
            && Random.value <= 0.33f)
        {
            GameObject copy = Instantiate(survivor.gameObject, survivor.transform.parent);
            copy.transform.localPosition = survivor.transform.localPosition + new Vector3(20f, 0f, 0f);
            Debug.Log($"{gameObject.name}: Blender Productivity triggered — extra {resultData.Name} produced.");
        }

        // Blender Quality: start post-blend rest timer
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.HasUpgrade("blender_quality"))
        {
            _isPostBlending = true;
            _postBlendTimer = 0f;
            if (_timerFill != null) _timerFill.fillAmount = 0f;
            Debug.Log($"{gameObject.name}: Blender Quality rest period started.");
        }

        Debug.Log($"{gameObject.name}: Blended! → {resultData.Name}");
    }

    private void TurnIntoSlop()
    {
        IngredientData slop = IngredientLookup.Get(RecipeManager.SlopResult);

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

        SpoilageTriggerManager.Trigger(SpoilageCategory.HUNGER);

        Debug.Log($"{gameObject.name}: Invalid blend, turned into Slop.");
    }

    private void DestroyExtraIngredients(IngredientObject survivor)
    {
        foreach (IngredientObject food in _currentFoods)
        {
            if (food != null && food != survivor)
                Destroy(food.gameObject);
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
}
