using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CookingStation : MonoBehaviour
{
    [Header("Base Station Settings")]

    // These properties are used to change the station's appearance when being used
    [SerializeField] private Image _stationImage;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _activeSprite;
    [SerializeField] private RectTransform _foodAnchor;



  
    protected int maxIngredients = 1;
    protected List<IngredientObject> _currentFoods = new List<IngredientObject>();
    protected List<IngredientBehaviour> _currentBehaviours = new List<IngredientBehaviour>();

    protected IngredientObject _currentFood => _currentFoods.Count > 0 ? _currentFoods[0] : null;
    protected IngredientBehaviour _currentIngredientBehaviour => _currentBehaviours.Count > 0 ? _currentBehaviours[0] : null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        _stationImage.sprite = _defaultSprite;
    }
    

    public bool CanAcceptFood(FoodGrab food)
    {
    return food != null && _currentFoods.Count < maxIngredients;
    }

    public void PlaceFood(FoodGrab food)
    {
        if (!CanAcceptFood(food))
        return;

        RectTransform foodRect = food.GetComponent<RectTransform>();

        if (foodRect != null && _foodAnchor != null)
        {
            foodRect.SetParent(_foodAnchor, false);
            foodRect.anchoredPosition = Vector2.zero;
            foodRect.SetAsLastSibling();

        }

        
        IngredientObject ingredient = food.GetComponent<IngredientObject>();
        IngredientBehaviour behaviour = food.GetComponent<IngredientBehaviour>();

        if (ingredient != null && !_currentFoods.Contains(ingredient))
        _currentFoods.Add(ingredient);

        if (behaviour != null && !_currentBehaviours.Contains(behaviour))
        _currentBehaviours.Add(behaviour);

        SetSpriteActive(true);

        OnPlaceFood(food);
        
    }

    public virtual void OnPlaceFood(FoodGrab food){}

    public virtual void RemoveFood(FoodGrab food)
    {
        OnRemoveFood();
        
        IngredientObject ingredient = food.GetComponent<IngredientObject>();
        IngredientBehaviour behaviour = food.GetComponent<IngredientBehaviour>();

        if (ingredient != null)
            _currentFoods.Remove(ingredient);

        if (behaviour != null)
            _currentBehaviours.Remove(behaviour);

        SetSpriteActive(_currentFoods.Count > 0);

    }

    public virtual void OnRemoveFood()
    {
        _currentFoods.Clear();
        _currentBehaviours.Clear();
        SetSpriteActive(false);
    }

    public virtual void OnRemoveFood(FoodGrab food)
    {
        RemoveFood(food);
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
