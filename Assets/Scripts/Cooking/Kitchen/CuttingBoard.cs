using UnityEngine;
using UnityEngine.InputSystem;

public class CuttingBoard : ManualStation
{
    [SerializeField] private IngredientData _requiredInput;
    [SerializeField] private IngredientData _outputIngredient;

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

        if (_requiredInput != null && _currentFood.IngredientInstance.Data != _requiredInput)
        {
            Debug.Log("Wrong ingredient for cutting board");
            return;
        }
    }

    public override void OnAction()
    {
        Debug.Log($"Action triggered on {gameObject.name}. Current Food: {(_currentFood != null ? _currentFood.name : "NULL")}");
        if (_currentFood == null || _currentFood.IngredientInstance == null) return;

        if (_requiredInput != null && _currentFood.IngredientInstance.Data != _requiredInput)
        {
            return;
        }

        if (_outputIngredient == null)
        {
            Debug.LogWarning("Output ingredient is missing on " + gameObject.name);
            return;
        }

        base.OnAction();

        Debug.Log("Chopping Food");

        if (_currentClicks >= _clicksPerState)
        {
            _currentFood.ChangeIngredient(_outputIngredient);
            Debug.Log("Food is now " + _outputIngredient.Name);

            _currentClicks = 0; // reset for next use
        }
    }
}