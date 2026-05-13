using UnityEngine;
using UnityEngine.UI;

public sealed class IngredientBehaviour : MonoBehaviour
{
    [SerializeField] private bool _enableDebugLogs = true;

    [Header("Spoiling Timer UI")]
    [SerializeField] private GameObject _spoilingTimer;
    [SerializeField] private Image _spoilingTimerFill;

    private IngredientObject _ingredientObject;

    private bool _isOnSpoilSurface;

    private void Awake()
    {
        _ingredientObject = GetComponent<IngredientObject>();

        if (_spoilingTimer == null)
        {
            Transform timer = transform.Find("Canvas/SpoilTimer");
            if (timer != null)
            {
                _spoilingTimer = timer.gameObject;
            }
        }

        if (_spoilingTimerFill == null)
        {
            Transform fill = transform.Find("Canvas/SpoilTimer/SpoilFill");
            if (fill != null)
            {
                _spoilingTimerFill = fill.GetComponent<Image>();
            }
        }

        HideSpoilingTimer();
    }

    private void Update()
    {
        HandleSpoilage();
        UpdateSpoilageVisual();
    }

    public void PlateIngredient()
    {
        _ingredientObject.IngredientInstance.IsPlated = true;
    }
    private void HandleSpoilage()
    {
        Ingredient ingredient = _ingredientObject.IngredientInstance;

        if (ingredient == null) return;
        if (ingredient.IsSpoiled) return;
        if (!_isOnSpoilSurface) return;
        if (ingredient.Data.SpoilTime <= 0f) return;

        float percentPerSecond = 100f / ingredient.Data.SpoilTime;
        ingredient.AddSpoilagePercent(percentPerSecond * Time.deltaTime);

        if (ingredient.IsSpoiled)
        {
            Log("Food reached 100% spoilage.");
        }
    }

    private void UpdateSpoilageVisual()
    {
        Ingredient ingredient = _ingredientObject.IngredientInstance;

        if (ingredient == null) return;

        if (!_isOnSpoilSurface || ingredient.IsSpoiled)
        {
            HideSpoilingTimer();
            return;
        }

        float remainingPercent = 1f - ingredient.SpoilagePercent / 100f;
        ShowSpoilingTimer(remainingPercent);
    }

    public void PutOnSpoilSurface()
    {
        _isOnSpoilSurface = true;
        Log("Started spoiling.");
    }

    public void RemoveFromSpoilSurface()
    {
        _isOnSpoilSurface = false;
        HideSpoilingTimer();
        Log("Stopped spoiling.");
    }

    private void ShowSpoilingTimer(float fillAmount)
    {
        if (_spoilingTimer == null || _spoilingTimerFill == null) return;

        _spoilingTimer.SetActive(true);
        _spoilingTimerFill.fillAmount = Mathf.Clamp01(fillAmount);
    }

    private void HideSpoilingTimer()
    {
        if (_spoilingTimer != null)
        {
            _spoilingTimer.SetActive(false);
        }
    }

    private void Log(string message)
    {
        if (_enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] {message}");
        }
    }
}