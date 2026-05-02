using UnityEngine;

public class SeasoningStation : UtilityStation
{

    private DishObject _currentFood;
    private int _counter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _counter = 3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Maybe don't use foodgrab food, also make new dish object to operate on and replace all the IngredientIntance/State
    public override void OnPlaceFood(UtilityGrab dish)
    {
         _currentFood = dish.GetComponent<DishObject>();

        if (_currentFood == null)
        {
            Debug.LogWarning("No Dish found!");
            return;
        }

        Debug.Log("Food on Station");

        if (_currentFood.DishInstance.CurrentState == DishState.Seasoned)
        {
            Debug.Log("Food is already seasoned");
            return; 
        }

        SeasonFood();

    }

    public void SeasonFood()
    {
        _counter--;
        Debug.Log("Seasoning, " + _counter + " uses left.");
    }

     public override void OnRemoveFood()
    /*{
        _currentFood = null;
    }*/
    {
        if (_currentFood == null) return;

        _currentFood.DishInstance.CurrentState = DishState.Seasoned;

        Debug.Log("Food is now Seasoned");

        _currentFood = null;
    }
}
