using UnityEngine;

public class ToasterOven : CookingAppliance
{
    [SerializeField] private Timer _timer;

    private IngredientObject _currentFood;
    private bool _isCooking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the GameConsole object in the scene and get its Timer component
        _timer = GameObject.Find("GameConsole").GetComponent<Timer>();
    }


    public override void OnPlaceFood(FoodGrab food)
    {
        _currentFood = food.GetComponent<IngredientObject>();

        if (_currentFood == null)
        {
            Debug.LogWarning("No IngredientObject found!");
            return;
        }

        Debug.Log("Food in Toaster Oven");

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

    public override void OnRemoveFood()
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
        _currentFood.IngredientInstance.CurrentCookState = targetState;
        Debug.Log(_currentFood.IngredientInstance.Data.Name + " is now Cooked!");
        Debug.Log($"Cooking Finished! Result is {targetState}");

        string foodName = _currentFood.IngredientInstance.Data.Name;

        if (foodName == "Dough")
        {
            if (targetState == CookState.Toasted)
            {
                Debug.Log("The Dough has become TOAST!");
                // Later we will add: _currentFood.IngredientInstance.Data.Name = "Toast";
            }
            else if (targetState == CookState.Grilled)
            {
                Debug.Log("The Dough has become FLATBREAD!");
            }
        }
    }
}
