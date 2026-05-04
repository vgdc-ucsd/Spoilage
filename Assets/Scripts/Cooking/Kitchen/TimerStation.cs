using UnityEngine;

public class TimerStation : CookingStation
{
    // IMPORTANT NOTE PLEASE READ
    // This class will not work as intended until the timer has been implemented in the scene. For now, 
    // all references to _timer have been commented out to prevent null reference errors

    // [SerializeField] private Timer _timer;

    // This basically tells us the station if it's a toaster (Toasted), grill (Grilled), etc.
    [SerializeField] private CookState _targetState;
    // If true, the station can continue cooking after reaching the target state
    [SerializeField] private bool _canOvercook = false;
    [SerializeField] private CookState _overcookedState;
    private bool _isCooking = false;

    public override void Start()
    {
        base.Start();

        // Find the GameConsole object in the scene and get its Timer component
        // _timer = GameObject.Find("GameConsole").GetComponent<Timer>();
    }

    public override void OnPlaceFood(FoodGrab food)
    {
        base.OnPlaceFood(food);

        // execute any special logic after calling base to ensure the food is set before accessing it

        // SAFETY CHECK: Ensure food and timer exist
        if (_currentFood == null || _currentIngredientBehaviour == null)
        {
            Debug.LogWarning("Missing IngredientObject or IngredientBehaviour reference!");
            return;
        }

        // SAFETY CHECK: Ensure the ingredient data was assigned in the Inspector
        if (_currentFood.IngredientInstance == null || _currentFood.IngredientInstance.Data == null)
        {
            Debug.LogError("Food data is missing on " + _currentFood.name);
            return;
        }

        // If the timer has time left and it's less than the total cook time, resume it.
        // if (_timer.TimeRemaining > 0 && _timer.TimeRemaining < _currentFood.IngredientInstance.Data.CookTime)
        // {
        //     _isCooking = true;
        //     _timer.ResumeTimer();
        //     Debug.Log("Resuming timer at: " + _timer.TimeRemaining);
        // }
        // else
        // {
            StartCooking();
        // }
    }

    public override void OnRemoveFood()
    {
        // execute any special logic before calling base to ensure the food is still accessible if needed

        if (_currentFood == null)
        {
            return;
        }

        if (_currentIngredientBehaviour != null)
        {
            _currentIngredientBehaviour.RemoveFromHeat();
        }

        if (_isCooking)
        {
            _isCooking = false;
            // _timer.PauseTimer();
            // Debug.Log("Timer paused.");
        }

        _isCooking = false;

        base.OnRemoveFood();
    }

    public virtual void StartCooking()
    {
        _isCooking = true;
        // _currentFood.IngredientInstance.CurrentCookState = CookState.Raw;
        _currentIngredientBehaviour.PutOnHeat();
        // _timer.StartTimer(_currentFood.IngredientInstance.Data.CookTime);
        Debug.Log("Started cooking");
    }

    public virtual void Update()
    {
        // if (!_isCooking || _currentFood == null || _timer == null) return;
        if (!_isCooking || _currentFood == null) return;

        // if (_timer.IsFinished())
        // {
        //     if (_canOvercook && _currentFood.IngredientInstance.CurrentCookState == _targetState)
        //     {
        //         FinishOvercooking();
        //         Debug.Log("Food overcooked!");
        //     }
        //     else
        //     {
        //         FinishCooking();
        //         Debug.Log("Food cooked!");
        //     }
        // }
    }

    public virtual void FinishCooking()
    {
        _isCooking = false;
        if (_currentFood != null)
        {
            _currentFood.IngredientInstance.CurrentCookState = _targetState;
            Debug.Log($"Cooking Finished! {_currentFood.IngredientInstance.Data.Name} is now {_targetState}!");
        }
    }

    public virtual void FinishOvercooking()
    {
        _isCooking = false;
        if (_currentFood != null)
        {
            _currentFood.IngredientInstance.CurrentCookState = _overcookedState;
            Debug.Log($"Food overcooked! {_currentFood.IngredientInstance.Data.Name} is now {_overcookedState}!");
        }
    }
}
