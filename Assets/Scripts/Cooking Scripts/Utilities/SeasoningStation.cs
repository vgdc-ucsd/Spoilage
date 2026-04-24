using UnityEngine;

public class SeasoningStation : UtilityStation
{

    private IngredientObject _currentFood;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Maybe don't use foodgrab food, also make new dish object to operate on and replace all the IngredientIntance/State
    public override void OnPlaceFood(FoodGrab food)
    {
        // _currentFood = food.GetComponent<Dish>();

        if (_currentFood == null)
        {
            Debug.LogWarning("No Dish found!");
            return;
        }

        Debug.Log("Food on Station");

        // if (_currentFood.IngredientInstance.CurrentState == IngredientState.Seasoned)
        // {
        //     Debug.Log("Food is already seasoned");
        //     return; 
        // }

        SeasonFood();

    
        // _currentFood.IngredientInstance.CurrentState = IngredientState.Seasoned;
        // Debug.Log(_currentFood.IngredientInstance.Data.Name + " is now Seasoned!");
    }

    public void SeasonFood()
    {
        // _currentFood.IngredientInstance.CurrentState = IngredientState.Unseasoned;
        Debug.Log("Seasoning");
    }

     public override void OnRemoveFood()
    {
        _currentFood = null;
    }
}
