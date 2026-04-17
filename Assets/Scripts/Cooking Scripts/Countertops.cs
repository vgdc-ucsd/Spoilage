using UnityEngine;

public class Countertops : MonoBehaviour
{
    [SerializeField] private Timer _timer;
    private IngredientObject _currentFood;
    private bool _isSpoiling = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isSpoiling || _currentFood == null) return;

        if (_timer.IsFinished())
        {
            FinishSpoiling();
        }
    }

    public void OnPlaceFood(FoodGrab food)
    {
        _currentFood = food.GetComponent<IngredientObject>();

        if (_currentFood == null)
        {
            Debug.LogWarning("No IngredientObject found!");
            return;
        }

        Debug.Log("Food on Countertop");

        if (_currentFood.IngredientInstance.Data.IsSpoiled)
        {
            Debug.Log("Food is already spoiled");
            return; 
        }

        if (_timer.TimeRemaining > 0 && _timer.TimeRemaining < _currentFood.IngredientInstance.Data.SpoilTime)
        {
            _isSpoiling = true;
            _timer.ResumeTimer();
            Debug.Log("Resuming timer at: " + _timer.TimeRemaining);
        }

        else
        {
            SpoilFood();
        }

    }

    public void SpoilFood()
    {
        _isSpoiling = true;
        _currentFood.IngredientInstance.Data.IsSpoiled = false;
        _timer.StartTimer(_currentFood.IngredientInstance.Data.SpoilTime);
        Debug.Log("Started spoiling");
    }

    public void OnRemoveFood()
    {
        if (_isSpoiling)
        {
            _isSpoiling = false;
            _timer.PauseTimer();
            Debug.Log("Timer paused.");
        }
        _currentFood = null;
    }

    private void FinishSpoiling()
    {
        _isSpoiling = false;
        _currentFood.IngredientInstance.Data.IsSpoiled = true;
        Debug.Log(_currentFood.IngredientInstance.Data.Name + " is now Spoiled :(");
    }
}
