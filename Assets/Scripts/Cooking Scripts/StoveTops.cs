using UnityEngine;

public class StoveTops : CookingAppliance
{
    [SerializeField] private Timer _timer;

    private IngredientObject _currentFood;
    private bool _isCooking = false;

    public override void OnPlaceFood(FoodGrab food)
    {
        _currentFood = food.GetComponent<IngredientObject>();

        // SAFETY CHECK: Ensure food and timer exist
        if (_currentFood == null || _timer == null)
        {
            Debug.LogWarning("Missing IngredientObject or Timer reference!");
            return;
        }

        // SAFETY CHECK: Ensure the ingredient data was assigned in the Inspector
        if (_currentFood.IngredientInstance == null || _currentFood.IngredientInstance.Data == null)
        {
            Debug.LogError("Food data is missing on " + _currentFood.name);
            return;
        }

        Debug.Log("Food on Grill");

        if (_currentFood.IngredientInstance.CurrentCookState == CookState.Cooked)
        {
            Debug.Log("Food is already cooked");
            return;
        }

        // Your original Logic: Resume timer if partially cooked, else start fresh
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
        if (_currentFood == null || _timer == null) return;

        _isCooking = true;
        _currentFood.IngredientInstance.CurrentCookState = CookState.Raw;
        _timer.StartTimer(_currentFood.IngredientInstance.Data.CookTime);
        Debug.Log("Started cooking");
    }

    public override void OnRemoveFood()
    {
        if (_isCooking && _timer != null)
        {
            _isCooking = false;
            _timer.PauseTimer();
            Debug.Log("Timer paused.");
        }
        _currentFood = null;
    }

    private void Update()
    {
        if (!_isCooking || _currentFood == null || _timer == null) return;

        if (_timer.IsFinished())
        {
            FinishCooking();
        }
    }

    private void FinishCooking()
    {
        _isCooking = false;
        if (_currentFood != null)
        {
            _currentFood.IngredientInstance.CurrentCookState = targetState; //changed from CookState.Cooked to targetState
            Debug.Log(_currentFood.IngredientInstance.Data.Name + " is now Cooked!");
            Debug.Log($"Cooking Finished! Result is {targetState}");
        }
    }
}
