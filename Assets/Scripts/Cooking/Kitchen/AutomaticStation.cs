using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AutomaticStation : CookingStation
{

    // [SerializeField] private Timer _timer;

    // This basically tells us the station if it's a toaster, grill, etc.
    [SerializeField] private IngredientTransform[] _transforms;

    
    [Header("Cooking Settings")]
    [SerializeField] private float _cookDuration = 5f;
    [SerializeField] private float _overcookDuration = 5f;

    [Header("Timer UI")]
    [SerializeField] private GameObject _timerObject;
    [SerializeField] private Image _timerFill;


    private float _timer;
    private bool _isCooking;
    private bool _isOvercooking;

    public override void Start()
    {
        base.Start();
        HideTimer();

        // Find the GameConsole object in the scene and get its Timer component
        // _timer = GameObject.Find("GameConsole").GetComponent<Timer>();
    }

    public override void OnPlaceFood(FoodGrab food)
    {
        base.OnPlaceFood(food);

        // execute any special logic after calling base to ensure the food is set before accessing it

        // SAFETY CHECK: Ensure food and timer exist
        if (_currentFood == null || _currentFood.IngredientInstance == null || _currentFood.IngredientInstance.Data == null)
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

        StartCooking();
    }

    public override void OnRemoveFood()
    {
        StopCooking();
        base.OnRemoveFood();
    }

    public void StartCooking()
    {
        _timer = 0f;
        _isCooking = true;
        _isOvercooking = false;

        ShowTimer();
        Debug.Log("Started cooking");
    }

    private void StopCooking()
    {
        _isCooking = false;
        _isOvercooking = false;
        _timer = 0f;

        HideTimer();
    }



    public virtual void Update()
    {
        if (!_isCooking || _currentFood == null) return;

        _timer += Time.deltaTime;

        float duration = _isOvercooking ? _overcookDuration : _cookDuration;
        UpdateTimer(duration);
    
        if (_timer < duration)
        {
            return;
        }

        if (_isOvercooking)
        {
            FinishOvercooking();
            return;
        }

        FinishCooking();
    }

    private void UpdateTimer(float duration)
    {
        if (_timerFill == null)
        {
            return;
        }

        float progress = _timer / duration;
        _timerFill.fillAmount = Mathf.Clamp01(1f - progress);

        if (_isOvercooking)
        {
            _timerFill.color = Color.red;
        }
    }

    private void ShowTimer()
    {
        if (_timerObject != null)
        {
            _timerObject.SetActive(true);
        }
    }

    private void HideTimer()
    {
        if (_timerObject != null)
        {
            _timerObject.SetActive(false);
        }
    }


    public virtual void FinishCooking()
    {
        if (_currentFood == null || _currentFood.IngredientInstance == null)
        {
            StopCooking();
            return;
        }

        IngredientData currentData = _currentFood.IngredientInstance.Data;

        if (!TryGetTransform(currentData, out IngredientTransform transform))
        {
            Debug.LogWarning($"{gameObject.name} has no transform for {currentData.Name}");
            StopCooking();
            return;
        }

        if (transform.output == null)
        {
            Debug.LogError("Output ingredient is missing on " + gameObject.name);
            StopCooking();
            return;
        }

        _currentFood.ChangeIngredient(transform.output);
        Debug.Log($"Cooking finished! {currentData.Name} is now {transform.output.Name}!");

        if (transform.canOvercook && transform.overcookedOutput != null)
        {
            _timer = 0f;
            _isCooking = true;
            _isOvercooking = true;
            return;
        }
        StopCooking();
    }

    public virtual void FinishOvercooking()
    {

        if (_currentFood == null || _currentFood.IngredientInstance == null)
        {
            StopCooking();
            return;
        }

        IngredientData currentData = _currentFood.IngredientInstance.Data;

        if (!TryGetOvercookTransform(currentData, out IngredientTransform transform))
        {
            Debug.LogWarning($"{gameObject.name} has no overcook transform for {currentData.Name}");
            StopCooking();
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
        
        StopCooking();
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
