using UnityEngine;
using UnityEngine.InputSystem;

public class AutomaticStation : CookingStation
{
    // IMPORTANT NOTE PLEASE READ
    // This class will not work as intended until the timer has been implemented in the scene. For now, 
    // all references to _timer have been commented out to prevent null reference errors

    // [SerializeField] private Timer _timer;

    // This basically tells us the station if it's a toaster, grill, etc.
    [SerializeField] private IngredientTransform[] _transforms;

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

        IngredientData currentData = _currentFood.IngredientInstance.Data;

        if (!TryGetTransform(currentData, out IngredientTransform transform) &&
            !TryGetOvercookTransform(currentData, out transform))
        {
            Debug.Log($"{gameObject.name} cannot process {currentData.Name}");
            return;
        }

        if (transform.output == null)
        {
            Debug.LogError("Output ingredient is missing on " + gameObject.name);
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

        // _timer.StartTimer(_currentFood.IngredientInstance.Data.CookTime);
        Debug.Log("Started cooking");
    }

    // public virtual void Update()
    // {
    //     if (!_isCooking || _currentFood == null || _timer == null) return;
    //     if (!_isCooking || _currentFood == null) return;

    //     if (_timer.IsFinished())
    //     {
    //         if (_canOvercook && _currentFood.IngredientInstance.Data == _outputIngredient)
    //         {
    //             FinishOvercooking();
    //             Debug.Log("Food overcooked!");
    //         }
    //         else
    //         {
    //             FinishCooking();
    //             Debug.Log("Food cooked!");
    //         }
    //     }
    // }

    // TEMP TEST: press T to  finish cooking
    public virtual void Update()
    {
        if (!_isCooking || _currentFood == null) return;

        // TEMP TEST: press T to instantly finish cooking
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            IngredientData currentData = _currentFood.IngredientInstance.Data;

            if (TryGetOvercookTransform(currentData, out IngredientTransform transform) &&
                transform.canOvercook &&
                transform.overcookedOutput != null)
            {
                FinishOvercooking();
                Debug.Log("Food overcooked!");
            }
            else
            {
                FinishCooking();
                Debug.Log("Food cooked by temp T input!");
            }
        }
    }

    public virtual void FinishCooking()
    {
        if (_currentFood != null)
        {
            IngredientData currentData = _currentFood.IngredientInstance.Data;

            if (!TryGetTransform(currentData, out IngredientTransform transform))
            {
                Debug.LogWarning($"{gameObject.name} has no transform for {currentData.Name}");
                _isCooking = false;
                return;
            }

            if (transform.output == null)
            {
                Debug.LogError("Output ingredient is missing on " + gameObject.name);
                _isCooking = false;
                return;
            }

            _currentFood.ChangeIngredient(transform.output);
            Debug.Log($"Cooking Finished! {currentData.Name} is now {transform.output.Name}!");

            _isCooking = transform.canOvercook && transform.overcookedOutput != null;
        }
    }

    public virtual void FinishOvercooking()
    {
        _isCooking = false;

        if (_currentFood != null)
        {
            IngredientData currentData = _currentFood.IngredientInstance.Data;

            if (!TryGetOvercookTransform(currentData, out IngredientTransform transform))
            {
                Debug.LogWarning($"{gameObject.name} has no overcook transform for {currentData.Name}");
                return;
            }

            if (!transform.canOvercook)
            {
                return;
            }

            if (transform.overcookedOutput == null)
            {
                Debug.LogWarning("Overcooked ingredient is missing on " + gameObject.name);
                return;
            }

            _currentFood.ChangeIngredient(transform.overcookedOutput);
            Debug.Log($"Food overcooked! {currentData.Name} is now {transform.overcookedOutput.Name}!");
        }
    }

    private bool TryGetTransform(IngredientData input, out IngredientTransform matchingTransform)
    {
        foreach (IngredientTransform transform in _transforms)
        {
            if (transform.input == input)
            {
                matchingTransform = transform;
                return true;
            }
        }

        matchingTransform = null;
        return false;
    }

    private bool TryGetOvercookTransform(IngredientData input, out IngredientTransform matchingTransform)
    {
        foreach (IngredientTransform transform in _transforms)
        {
            if (transform.output == input)
            {
                matchingTransform = transform;
                return true;
            }
        }

        matchingTransform = null;
        return false;
    }
}