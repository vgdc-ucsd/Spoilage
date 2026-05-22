using UnityEngine;
using UnityEngine.UI;

public abstract class ManualStation : CookingStation
{
    [Header("Manual Settings")]
    [SerializeField] protected int _clicksPerState = 3;

    [Header("Manual UI")]
    [SerializeField] protected GameObject _timerObject;
    [SerializeField] protected Image _timerFill;
    [SerializeField] protected GameObject _actionButtonObject;

    protected int _currentClicks;
    protected bool _isActionComplete;

    public override void Start()
    {
        base.Start();
        HideManualUI();
    }
    
    public override bool OnPlaceFood(FoodGrab food)
    {
        bool placed = base.OnPlaceFood(food);

        _currentClicks = 0;
        _isActionComplete = false;

        ShowManualUI();
        UpdateTimer();

        return placed;
    }

    public override void OnRemoveFood()
    {
        _currentClicks = 0;
        _isActionComplete = false;

        HideManualUI();

        base.OnRemoveFood();
    }

    public virtual void OnAction()
    {
        if (_currentFood == null || _isActionComplete)
            return;

        _currentClicks++;
        UpdateTimer();

        if (_currentClicks >= _clicksPerState)
        {
            _isActionComplete = true;
            FillTimer();
            CompleteManualAction();
        }
    }

    protected abstract void CompleteManualAction();

    protected void UpdateTimer()
    {
        if (_timerFill == null || _clicksPerState <= 0) return;
        _timerFill.fillAmount = Mathf.Clamp01((float)_currentClicks / _clicksPerState);
    }

    protected void FillTimer()
    {
        if (_timerFill != null)
            _timerFill.fillAmount = 1f;
    }

    protected void ResetTimer()
    {
        _currentClicks = 0;
        _isActionComplete = false;
        UpdateTimer();
    }

    protected void ShowManualUI()
    {
        if (_timerObject != null) _timerObject.SetActive(true);
        if (_actionButtonObject != null) _actionButtonObject.SetActive(true);
    }

    protected void HideManualUI()
    {
        if (_timerObject != null) _timerObject.SetActive(false);
        if (_actionButtonObject != null) _actionButtonObject.SetActive(false);
    }
}