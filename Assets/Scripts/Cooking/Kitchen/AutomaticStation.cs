using UnityEngine;
using UnityEngine.UI;

public class AutomaticStation : CookingStation
{

    [Header("Cooking Settings")]
    [SerializeField] private float _cookDuration = 5f;
    [SerializeField] private CookState _targetState;
    [SerializeField] private bool _canOvercook;
    [SerializeField] private float _overcookDuration = 5f;
    [SerializeField] private CookState _overcookedState = CookState.Burnt;

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
    }

    private void StopCooking()
    {
        _isCooking = false;
        _isOvercooking = false;
        _timer = 0f;

        HideTimer();
    }

    public  void Update()
    {
        // if (!_isCooking || _currentFood == null || _timer == null) return;
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
        _currentFood.IngredientInstance.CurrentCookState = _targetState;

        if (_canOvercook)
        {
            _timer = 0f;
            _isOvercooking = true;
            return;
        }

        StopCooking();
    }

    public virtual void FinishOvercooking()
    {
        _currentFood.IngredientInstance.CurrentCookState = _overcookedState;
        Debug.Log($"Food overcooked! {_currentFood.IngredientInstance.Data.Name} is now {_overcookedState}!");
        
    }
}
