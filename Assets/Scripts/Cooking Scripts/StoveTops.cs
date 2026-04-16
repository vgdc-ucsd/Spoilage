using UnityEngine;

public class StoveTops : MonoBehaviour
{
    [SerializeField] private Timer _timer;

    private IngredientObject _currentFood;
    private bool _isCooking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnPlaceFood(FoodGrab food)
    {
        _currentFood = food.GetComponent<IngredientObject>();

        if (_currentFood == null)
        {
            Debug.LogWarning("No IngredientObject found!");
            return;
        }

        //food.transform.position = transform.position;
        Debug.Log("Food on Grill");

        if (_currentFood.IngredientInstance.CurrentState == IngredientState.Cooked)
        {
            Debug.Log("Food is already cooked");
            return; 
        }

        _isCooking = true;
        _currentFood.IngredientInstance.CurrentState = IngredientState.Raw;
        _timer.StartTimer(_currentFood.IngredientInstance.Data.CookTime);
        Debug.Log("Started cooking");
    }

    private void Update()
    {
        if (!_isCooking || _currentFood == null) return;

        if (_timer.IsFinished())
        {
            FinishCooking();
            UpdateCookedFoodSprite();
        }

    }

    private void FinishCooking()
    {
        _isCooking = false;
        _currentFood.IngredientInstance.CurrentState = IngredientState.Cooked;
        Debug.Log(_currentFood.IngredientInstance.Data.Name + " is now Cooked!");
    }

    private void UpdateCookedFoodSprite()
    {
        SpriteRenderer foodRenderer = _currentFood.GetComponent<SpriteRenderer>();

        Sprite cookedArt = _currentFood.IngredientInstance.Data.CookedSprite;

        if (foodRenderer != null && cookedArt != null)
        {
            foodRenderer.sprite = cookedArt;
            Debug.Log("Sprite updated to Cooked!");
        }
    }

}
