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

        Debug.Log("Food on Grill");

        if (_currentFood.IngredientInstance.CurrentCookState == CookState.Cooked)
        {
            Debug.Log("Food is already cooked");
            return; 
        }

        // If the timer has time left and it's less than the total cook time, resume it.
        if (_timer.TimeRemaining > 0 && _timer.TimeRemaining < _currentFood.IngredientInstance.Data.CookTime)
        {
            _isCooking = true;
            _timer.ResumeTimer();
            Debug.Log("Resuming timer at: " + _timer.TimeRemaining);
        }
        else
        {
            CookFood();
        }

    }

    public void CookFood()
    {

        _isCooking = true;
        _currentFood.IngredientInstance.CurrentCookState = CookState.Raw;
        _timer.StartTimer(_currentFood.IngredientInstance.Data.CookTime);
        Debug.Log("Started cooking");
        
    }

    public void OnRemoveFood()
    {
        if (_isCooking)
        {
            _isCooking = false;
            _timer.PauseTimer();
            Debug.Log("Timer paused.");
        }
        _currentFood = null;
    }

    private void Update()
    {
        if (!_isCooking || _currentFood == null) return;

        if (_timer.IsFinished())
        {
            FinishCooking();
            //UpdateCookedFoodSprite();
        }

    }

    private void FinishCooking()
    {
        _isCooking = false;
        _currentFood.IngredientInstance.CurrentCookState = CookState.Cooked;
        Debug.Log(_currentFood.IngredientInstance.Data.Name + " is now Cooked!");
    }

}
