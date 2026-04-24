/*
using UnityEngine;

public class CuttableFood : MonoBehaviour
{
    [Header("Cut Settings")]
    [SerializeField]
    private int _clicksPerState = 3;
    
    [SerializeField]
    private IngredientObject _currentFood;

    private int _currentClicks;
    private int _currentState;

    private void Start()
    {
        // Debug.Log($"[CuttableFood] Start — clicks per state: {_clicksPerState}");
        // Debug.Log($"[CuttableFood] Total states: {_stateObjects?.Length}");
        // UpdateVisualState();
    }

    private void OnMouseDown() 
    {
        Debug.Log("[CuttableFood] Mouse Clicked");
        Cut();
    }

    private void OnPlaceFood(FoodGrab food)
    {
        _currentFood = food.GetComponent<IngredientObject>();
        if (_currentFood == null)
        {
            Debug.LogWarning("No IngredientObject found!");
            return;
        }

        Debug.Log("Food on Cuttingboard");
        if (_currentFood.IngredientInstance.CurrentCookState == ChoppedState.Chopped)
        {
            Debug.Log("Food is already cooked");
            return; 
        }
    }

    private void Cut() 
    {
        if(_stateObjects == null || _stateObjects.Length == 0)
        {
            return;
        }

        _currentClicks++;
        Debug.Log($"[CuttableFood] Click {_currentClicks}/{_clicksPerState}");

        if(_currentClicks < _clicksPerState)
        {
            return;
        }

        _currentClicks = 0;
        _currentState++;

        Debug.Log($"[CuttableFood] ADVANCING TO STATE: {_currentState}");

        if (_currentState >= _stateObjects.Length)
        {
            Debug.Log("[CuttableFood] Reached final state, clamping.");
            _currentState = _stateObjects.Length - 1;
        }

        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if(_stateObjects == null || _stateObjects.Length == 0)
        {
            return;
        }

        Debug.Log($"[CuttableFood] Updating Visual State → {_currentState}");

        for(int i = 0; i < _stateObjects.Length; i++)
        {
            GameObject stateObject = _stateObjects[i];

            if(stateObject == null)
            {
                continue;
            }

            bool active = i == _currentState;
            stateObject.SetActive(active);

            Debug.Log($"[CuttableFood] State {i} → {(active ? "ACTIVE" : "hidden")} ({stateObject.name})");
        }
    }
}
*/