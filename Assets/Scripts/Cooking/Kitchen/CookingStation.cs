using UnityEngine;
using UnityEngine.UI;

public class CookingStation : MonoBehaviour
{
    [Header("Base Station Settings")]

    // These properties are used to change the station's appearance when being used
    [SerializeField] private Image _stationImage;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _activeSprite;

    protected IngredientObject _currentFood;
    protected IngredientBehaviour _currentIngredientBehaviour;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        _stationImage.sprite = _defaultSprite;
    }

    // These virtual methods allow child scripts to add their own unique logic
    public virtual void OnPlaceFood(FoodGrab food)
    {
        Debug.Log($"{gameObject.name}: Food placed.");
        SetSpriteActive(true);
        _currentFood = food.GetComponent<IngredientObject>();
        _currentIngredientBehaviour = food.GetComponent<IngredientBehaviour>();
    }

    public virtual void OnRemoveFood()
    {
        Debug.Log($"{gameObject.name}: Food removed.");
        SetSpriteActive(false);
        _currentFood = null;
        _currentIngredientBehaviour = null;
    }

    public void SetSpriteActive(bool isActive)
    {
        if (_stationImage == null)
        {
            Debug.LogWarning("Image reference is missing!");
            return;
        }

        // don't change sprites for work stations that don't have both sprites assigned
        if (_activeSprite == null || _defaultSprite == null)
        {
            return;
        }

        _stationImage.sprite = isActive ? _activeSprite : _defaultSprite;
    }
}
