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
    [Header("Spoilage Timer UI")]
    [SerializeField] private GameObject _spoilingTimerObject;
    [SerializeField] private Image _spoilTimerFill;
    private bool _isSpoiling;

    private IngredientObject _ingredientObject;

    private float _spoilTimer;

    private void Awake()
    {
        _ingredientObject = GetComponent<IngredientObject>();
        HideSpoilTimer();
    }

    private void Update()
    {
        HandleSpoilage();
    }

    public void PutOnSpoilSurface()
    {
        if (_ingredientObject == null || _ingredientObject.IngredientInstance == null)
        {
            return;
        }
        if (_ingredientObject.IngredientInstance.IsSpoiled)
        {
            return;
        }
        _isSpoiling = true;
        ShowSpoilTimer();
    }

    public void RemoveFromSpoilSurface()
    {
        _isSpoiling = false;
    }

    private void HandleSpoilage()
    {
        if (!_isSpoiling || _ingredientObject == null || _ingredientObject.IngredientInstance == null)
        {
            return;
        }

        Ingredient ingredient = _ingredientObject.IngredientInstance;

        if (ingredient.IsSpoiled)
        {
            HideSpoilTimer();
            return;
        }
        _spoilTimer += Time.deltaTime;

        float spoilTime = ingredient.Data.SpoilTime;

        if (spoilTime <= 0f)
        {
            ingredient.IsSpoiled = true;
            ingredient.SpoilagePercent = 1f;
            HideSpoilTimer();
            return;
        }

        ingredient.SpoilagePercent = Mathf.Clamp01(_spoilTimer / spoilTime);
        UpdateSpoilTimer(ingredient.SpoilagePercent);

        if (ingredient.SpoilagePercent >= 1f)
        {
            ingredient.IsSpoiled = true;
            HideSpoilTimer();
        }
    }

    private void UpdateSpoilTimer(float spoilagePercent)
    {
        if (_spoilTimerFill == null)
        {
            return;
        }
        _spoilTimerFill.fillAmount = Mathf.Clamp01(1f - spoilagePercent);
    }

    private void ShowSpoilTimer()
    {
        if (_spoilingTimerObject != null)
        {
            _spoilingTimerObject.SetActive(true);
        }
    }

    private void HideSpoilTimer()
    {
        if (_spoilingTimerObject != null)
        {
            _spoilingTimerObject.SetActive(false);
        }
    }

    
}