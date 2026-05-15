using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CookingStation : MonoBehaviour, IPointerClickHandler
{
    [Header("Base Station Settings")]
    [SerializeField] private Image _stationImage;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _activeSprite;

    [SerializeField] protected string _station;

    [Header("Ingredient Capacity")]
    [SerializeField] protected int maxIngredients = 1;

    protected readonly List<IngredientObject> _currentFoods = new();
    protected readonly List<IngredientBehaviour> _currentBehaviours = new();

    protected IngredientObject _currentFood => _currentFoods.Count > 0 ? _currentFoods[0] : null;
    protected IngredientBehaviour _currentIngredientBehaviour => _currentBehaviours.Count > 0 ? _currentBehaviours[0] : null;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (FoodGrab.IsDeleteModeActive)
        {
            Debug.Log($"[Delete Mode] Station {gameObject.name} clicked. Deleting children.");

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                
                if (child.GetComponent<FoodGrab>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            OnRemoveFood();
        }
    }
    
    public bool HasSpace
    {
        get
        {
            _currentFoods.RemoveAll(f => f == null);
            return _currentFoods.Count < maxIngredients;
        }
    }
    public int CurrentFoodCount => _currentFoods.Count;
    public int MaxIngredients => maxIngredients;

    public virtual void Start()
    {
        SetSpriteActive(false);
    }

    public virtual bool OnPlaceFood(FoodGrab food)
    {
        if (food == null)
        {
            Debug.LogWarning($"{gameObject.name}: Tried to place null food.");
            return false;
        }

        if (!HasSpace)
        {
            Debug.LogWarning($"{gameObject.name}: Station is full. Max ingredients: {maxIngredients}");
            return false;
        }

        IngredientObject ingredient = food.GetComponent<IngredientObject>();
        IngredientBehaviour behaviour = food.GetComponent<IngredientBehaviour>();

        if (ingredient == null)
        {
            Debug.LogWarning($"{gameObject.name}: Placed object has no IngredientObject.");
            return false;
        }

        if (!_currentFoods.Contains(ingredient))
        {
            _currentFoods.RemoveAll(f => f == null);
            _currentBehaviours.RemoveAll(b => b == null);
            _currentFoods.Add(ingredient);
        }

        if (behaviour != null && !_currentBehaviours.Contains(behaviour))
        {
            _currentBehaviours.Add(behaviour);
        }

        Debug.Log($"{gameObject.name}: Food placed. Count: {_currentFoods.Count}/{maxIngredients}");
        SetSpriteActive(true);
        return true;
    }

    public virtual void OnRemoveFood()
    {
        Debug.Log($"{gameObject.name}: Food removed.");

        _currentFoods.Clear();
        _currentBehaviours.Clear();

        SetSpriteActive(false);
    }

    protected void ClearStationTracking()
    {
        _currentFoods.Clear();
        _currentBehaviours.Clear();
        SetSpriteActive(false);
    }

    public void SetSpriteActive(bool isActive)
    {
        if (_stationImage == null)
        {
            Debug.LogWarning($"{gameObject.name}: Station image reference is missing.");
            return;
        }

        if (_activeSprite == null || _defaultSprite == null)
        {
            return;
        }

        _stationImage.sprite = isActive ? _activeSprite : _defaultSprite;
    }
}