using UnityEngine;
using UnityEngine.UI;


/*
    IngrediantBehavior:
    Timer for each individual food ingrediant
        Cooking: Set timer for each cookable ingrediant
        Spoiling: Set timer for each ingrediant
*/
public sealed class IngredientBehaviour : MonoBehaviour
{

    [SerializeField] private bool _enableDebugLogs = true;
    [SerializeField] private GameObject _cookingTimer;
    [SerializeField] private Image _cookingTimerFill;

    [SerializeField] private GameObject _spoilingTimer;
    [SerializeField] private Image _spoilingTimerFill;

    private IngredientObject _ingredientObject;

    private float _cookStateTimer;

    private float _spoilTimer;

    private bool _isOnHeat;

    private bool _isOnSpoilSurface;

    private void Log(string message)
    {
        if (_enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] {message}");
        }
    }

    private void Start()
    {
        _ingredientObject = GetComponent<IngredientObject>();
    }

    private void Update()
    {
        HandleCookingTimers();
        HandleSpoilTimers();
        UpdateTimerVisuals();
    }

    private void HandleSpoilTimers()
    {
        Ingredient ingredient = _ingredientObject.IngredientInstance;

        if (!_isOnSpoilSurface || ingredient.IsSpoiled)
        {
            return;
        }

        _spoilTimer += Time.deltaTime;

        if (_spoilTimer >= ingredient.Data.SpoilTime)
        {
            ingredient.IsSpoiled = true;
            _spoilTimer = 0f;

            Log("Food SPOILED");
            HideSpoilingTimer();
        }
    }

    private void HandleCookingTimers()
    {
        Ingredient ingredient = _ingredientObject.IngredientInstance;

        if (!_isOnHeat || ingredient.IsSpoiled || !ingredient.Data.NeedsCooking)
        {
            return;
        }

        _cookStateTimer += Time.deltaTime;

        switch (ingredient.CurrentCookState)
        {
            case CookState.Raw:
                if (_cookStateTimer >= ingredient.Data.CookTime)
                {
                    ingredient.CurrentCookState = CookState.Cooked;
                    _cookStateTimer = 0f;

                    Log("Became COOKED");
                }
                break;

            case CookState.Cooked:
                if (_cookStateTimer >= ingredient.Data.CookTime)
                {
                    ingredient.CurrentCookState = CookState.Burnt;
                    _cookStateTimer = 0f;

                    Log("Became BURNT");
                    HideCookingTimer();
                }
                break;

            case CookState.Burnt:
                HideCookingTimer();
                break;
        }
    }

    private void UpdateTimerVisuals()
    {
        Ingredient ingredient = _ingredientObject.IngredientInstance;

        if (_isOnHeat && !ingredient.IsSpoiled && ingredient.Data.NeedsCooking && ingredient.CurrentCookState != CookState.Burnt)
        {
            float cookingProgress = _cookStateTimer / ingredient.Data.CookTime;
            ShowCookingTimer(1f - cookingProgress);
            HideSpoilingTimer();
            return;
        }
        if (_isOnSpoilSurface && !ingredient.IsSpoiled)
        {
            float spoilProgress = _spoilTimer / ingredient.Data.SpoilTime;
            ShowSpoilingTimer(1f - spoilProgress);
            HideCookingTimer();
            return;
        }
        if (ingredient.CurrentCookState == CookState.Raw && _cookStateTimer > 0f)
        {
            ShowCookingTimer(1f - (_cookStateTimer / ingredient.Data.CookTime));
            return;
        }

        HideCookingTimer();
        HideSpoilingTimer();
    }

    private void ShowCookingTimer(float fillAmount)
    {
        _cookingTimer.SetActive(true);
        _cookingTimerFill.fillAmount = Mathf.Clamp01(fillAmount);
    }

    private void HideCookingTimer()
    {
        _cookingTimer.SetActive(false);
    }

    private void ShowSpoilingTimer(float fillAmount)
    {
        _spoilingTimer.SetActive(true);
        _spoilingTimerFill.fillAmount = Mathf.Clamp01(fillAmount);
    }

    private void HideSpoilingTimer()
    {
        _spoilingTimer.SetActive(false);
    }


    public void PutOnHeat()
    {
        _isOnHeat = true;
        _isOnSpoilSurface = false;
        Log("Started Cooking");
    }

    public void RemoveFromHeat()
    {
        _isOnHeat = false;

        Ingredient ingredient = _ingredientObject.IngredientInstance;

        if (ingredient.CurrentCookState == CookState.Raw && _cookStateTimer > 0f)
        {
            ShowCookingTimer(1f - (_cookStateTimer / ingredient.Data.CookTime));
            Log($" ||Cooking Paused at {_cookStateTimer:F2}s");
            return;
        }

        HideCookingTimer();
    }

    public void PutOnSpoilSurface()
    {
        _isOnSpoilSurface = true;
        _isOnHeat = false;
        Log("Started Spoiling");
    }

    public void RemoveFromSpoilSurface()
    {
        _isOnSpoilSurface = false;
    }

}