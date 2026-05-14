using UnityEngine;
using UnityEngine.InputSystem;

public class CuttingBoard : ManualStation
{
    [SerializeField] private IngredientTransform[] _transforms;

    // TODO: delete this once popup button is implemented
    void Update()
    {
        if (_currentFood == null) return;

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame) // F key
        {
            OnAction();
        }
    }

    public override void OnPlaceFood(FoodGrab food)
    {
        base.OnPlaceFood(food);

        if (_currentFood == null || _currentFood.IngredientInstance == null) return;

        Debug.Log("Food on cutting board");

        IngredientData currentData = _currentFood.IngredientInstance.Data;

        if (!TryGetTransform(currentData, out IngredientTransform transform))
        {
            Debug.Log("Wrong ingredient for cutting board");
            return;
        }

        if (transform.output == null)
        {
            Debug.LogWarning("Output ingredient is missing on " + gameObject.name);
            return;
        }
    }

    public override void OnAction()
    {
        Debug.Log($"Action triggered on {gameObject.name}. Current Food: {(_currentFood != null ? _currentFood.name : "NULL")}");
        SpoilageTriggerManager.Instance.Invoke(SpoilageCategory.DISTRESS);
        if (_currentFood == null || _currentFood.IngredientInstance == null) return;

        IngredientData currentData = _currentFood.IngredientInstance.Data;

        if (!TryGetTransform(currentData, out IngredientTransform transform))
        {
            Debug.Log("Wrong ingredient for cutting board");
            return;
        }

        if (transform.output == null)
        {
            Debug.LogWarning("Output ingredient is missing on " + gameObject.name);
            return;
        }

        base.OnAction();

        Debug.Log("Chopping Food");

        if (_currentClicks >= _clicksPerState)
        {
            _currentFood.ChangeIngredient(transform.output);
            Debug.Log("Food is now " + transform.output.Name);

            _currentClicks = 0; // reset for next use
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
}