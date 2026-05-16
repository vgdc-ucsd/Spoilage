using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CuttingBoard : ManualStation
{
    [Header("Cutting Timer UI")]
    [SerializeField] private GameObject _timerObject;
    [SerializeField] private Image _timerFill;

    [Header("Cut Button UI")]
    [SerializeField] private GameObject _cutButtonObject;

    public override void Start()
    {
        maxIngredients = 1;
        base.Start();

        HideTimer();
        HideCutButton();
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
            return false;
        }

        if (!HasSpace)
        {
            Debug.LogWarning($"{gameObject.name}: Cutting board only accepts one ingredient.");
            return false;
        }

        base.OnPlaceFood(food);

        if (_currentFood == null || _currentFood.IngredientInstance == null)
        {
            return false;
        }

        _currentClicks = 0;

        ShowTimer();
        ShowCutButton();
        UpdateTimer();

        Debug.Log($"{gameObject.name}: Food on cutting board.");

        RecipeManager recipeManager = FindAnyObjectByType<RecipeManager>();

        if (recipeManager == null)
        {
            Debug.LogError($"{gameObject.name}: RecipeManager not found.");
            return false;
        }

        List<IngredientObject> check = new() { _currentFood };
        string result = recipeManager.CheckRecipe(check, _station);

        if (IsInvalidRecipeResult(result))
        {
            Debug.Log($"{gameObject.name}: Wrong ingredient for cutting board.");
        }

        return true;
    }

    public override void OnRemoveFood()
    {
        base.OnRemoveFood();

        _currentClicks = 0;
        UpdateTimer();

        HideTimer();
        HideCutButton();
    }

    public void PressCutButton()
    {
        Debug.Log($"{gameObject.name}: Cut button pressed.");
        OnAction();
    }

    public override void OnAction()
    {
        Debug.Log($"Action triggered on {gameObject.name}. Current Food: {(_currentFood != null ? _currentFood.name : "NULL")}");

        if (_currentFood == null || _currentFood.IngredientInstance == null)
        {
            return;
        }

        if (SpoilageTriggerManager.Instance != null)
        {
            SpoilageTriggerManager.Instance.Invoke(SpoilageCategory.DISTRESS);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: SpoilageTriggerManager.Instance is null.");
        }

        base.OnAction();

        UpdateTimer();

        Debug.Log($"{gameObject.name}: Click progress = {_currentClicks}/{_clicksPerState}");

        if (_currentClicks < _clicksPerState)
        {
            return;
        }

        RecipeManager recipeManager = FindAnyObjectByType<RecipeManager>();

        if (recipeManager == null)
        {
            Debug.LogError($"{gameObject.name}: RecipeManager not found.");
            ResetTimer();
            return;
        }

        List<IngredientObject> ingredients = new() { _currentFood };
        string resultName = recipeManager.CheckRecipe(ingredients, _station);

        if (IsInvalidRecipeResult(resultName))
        {
            Debug.Log($"{gameObject.name}: Wrong ingredient for cutting board.");
            ResetTimer();
            return;
        }

        IngredientData resultData = IngredientLookup.Get(resultName);

        if (resultData == null)
        {
            Debug.LogError($"{gameObject.name}: Could not find IngredientData for result '{resultName}'.");
            ResetTimer();
            return;
        }

        _currentFood.ChangeIngredient(resultData);

        Debug.Log($"{gameObject.name}: Chopped! → {resultData.Name}");

        FillTimer();

        // Keep the cut button visible while food is still on the board.
        // If you want the button to disappear after successful chopping, uncomment this:
        // HideCutButton();
    }

    private void UpdateTimer()
    {
        if (_timerFill == null)
        {
            return;
        }

        if (_clicksPerState <= 0)
        {
            return;
        }

        float progress = (float)_currentClicks / _clicksPerState;
        _timerFill.fillAmount = Mathf.Clamp01(progress);
        _timerFill.color = Color.green;
    }

    private void FillTimer()
    {
        if (_timerFill == null)
        {
            return;
        }

        _timerFill.fillAmount = 1f;
        _timerFill.color = Color.green;
    }

    private void ResetTimer()
    {
        _currentClicks = 0;
        UpdateTimer();
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

    private void ShowCutButton()
    {
        if (_cutButtonObject != null)
        {
            _cutButtonObject.SetActive(true);
        }
    }

    private void HideCutButton()
    {
        if (_cutButtonObject != null)
        {
            _cutButtonObject.SetActive(false);
        }
    }

    private bool IsInvalidRecipeResult(string resultName)
    {
        return string.IsNullOrEmpty(resultName)
            || resultName == "Slop"
            || resultName == "JSON Error";
    }
}