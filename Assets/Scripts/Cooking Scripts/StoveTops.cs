using UnityEngine;

public class StoveTops : CookingAppliance
{

    private IngredientObject _currentFood;

    void Start()
    {
        // Find the GameConsole object in the scene and get its Timer component
        _timer = GameObject.Find("GameConsole").GetComponent<Timer>();
    }
    public override void OnPlaceFood(FoodGrab food)
    {
        _currentFood = food.GetComponent<IngredientObject>();

        IngredientBehaviour ingredientBehaviour = food.GetComponent<IngredientBehaviour>();

        // SAFETY CHECK: Ensure food and timer exist
        if (_currentFood == null || ingredientBehaviour == null)
        {
            Debug.LogWarning("Missing IngredientObject or Timer reference!");
            return;
        }

        // SAFETY CHECK: Ensure the ingredient data was assigned in the Inspector
        if (_currentFood.IngredientInstance == null || _currentFood.IngredientInstance.Data == null)
        {
            Debug.LogError("Food data is missing on " + _currentFood.name);
            return;
        }

        Debug.Log("Food on Grill");

        ingredientBehaviour.PutOnHeat();
    }


    public override void OnRemoveFood()
    {
        if (_currentFood == null)
        {
            return;
        }

        IngredientBehaviour ingredientBehaviour = _currentFood.GetComponent<IngredientBehaviour>();

        if (ingredientBehaviour != null)
        {
            ingredientBehaviour.RemoveFromHeat();
            
        }
        Debug.Log("Food removed from stove");
        _currentFood = null;
    }

    private void Update()
    {
        if (!_isCooking || _currentFood == null || _timer == null) return;

        if (_timer.IsFinished())
        {
            FinishCooking();
        }
    }

    private void FinishCooking()
    {
        _isCooking = false;
        if (_currentFood != null)
        {
            _currentFood.IngredientInstance.CurrentCookState = targetState; //changed from CookState.Cooked to targetState
            Debug.Log(_currentFood.IngredientInstance.Data.Name + " is now Cooked!");
            Debug.Log($"Cooking Finished! Result is {targetState}");
        }
    }
}
