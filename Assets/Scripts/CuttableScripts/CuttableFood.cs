using UnityEngine;

public class CuttableFood : MonoBehaviour
{
    [Header("Cut Settings")]
    [SerializeField]
    private int _clicksPerState = 3;
    
    [SerializeField]
    private GameObject[] _stateObjects;

    private int _currentClicks;
    private int _currentState;

    private void Start()
    {
        Debug.Log($"[CuttableFood] Start — clicks per state: {_clicksPerState}");
        Debug.Log($"[CuttableFood] Total states: {_stateObjects?.Length}");
        UpdateVisualState();
    }

    private void OnMouseDown() 
    {
        Debug.Log("[CuttableFood] Mouse Clicked");
        Cut();
    }

    private void Cut() 
    {
        if(_stateObjects == null || _stateObjects.Length == 0)
        {
            Debug.LogWarning("[CuttableFood] No state objects assigned!");
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
            Debug.LogWarning("[CuttableFood] UpdateVisualState failed — no states");
            return;
        }

        Debug.Log($"[CuttableFood] Updating Visual State → {_currentState}");

        for(int i = 0; i < _stateObjects.Length; i++)
        {
            GameObject stateObject = _stateObjects[i];

            if(stateObject == null)
            {
                Debug.LogWarning($"[CuttableFood] State {i} is NULL");
                continue;
            }

            bool active = i == _currentState;
            stateObject.SetActive(active);

            Debug.Log($"[CuttableFood] State {i} → {(active ? "ACTIVE" : "hidden")} ({stateObject.name})");
        }
    }
}