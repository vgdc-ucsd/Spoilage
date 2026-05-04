using UnityEngine;
using UnityEngine.InputSystem;

public class CuttingBoard : ManualStation
{
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

        Debug.Log("Food on cutting board");

        if (_currentFood.IngredientInstance.CurrentChoppedState == ChoppedState.Chopped)
        {
            Debug.Log("Food is already chopped");
            return;
        }
    }

    public override void OnAction()
    {
        //dont cut if alr chopped 
        if (_currentFood.IngredientInstance.CurrentChoppedState == ChoppedState.Chopped)
        {
            return;
        }

        base.OnAction();

        Debug.Log("Chopping Food");

        if (_currentClicks >= _clicksPerState)
        {
            _currentFood.IngredientInstance.CurrentChoppedState = ChoppedState.Chopped;
            Debug.Log("Food is now chopped!");

            _currentClicks = 0; // reset for next use
        }
    }
}
