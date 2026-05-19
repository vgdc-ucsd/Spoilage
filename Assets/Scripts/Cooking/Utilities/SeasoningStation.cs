using UnityEngine;

public class SeasoningStation : UtilityStation
{

    private IngredientObject _currentIngredient;
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

        // Maybe don't use foodgrab food, also make new dish object to operate on 
        // and replace all the IngredientIntance/State
    public override void OnPlaceFood(FoodGrab dish)
    {
        _currentIngredient = dish.GetComponent<IngredientObject>();

        if (_currentIngredient == null)
        {
            Debug.LogWarning("No Ingredient found!");
            return;
        }

        Debug.Log("Food on Station");

        if (_currentIngredient.IsSeasoned)
        {
            Debug.Log("Ingredient is already seasoned");
            return; 
        }

        SeasonFood();

    }

    public void SeasonFood()
    {
        if(_currentIngredient == null) return;
        if(_counter > 0)
        {
            _counter--;
            _currentIngredient.SeasonIngredient();
            Debug.Log("Seasoning, " + _counter + " uses left.");
        }
        else
        {
            Debug.Log("No more seasoning left");
        }
    }

     public override void OnRemoveFood()
    {
        if (_currentIngredient == null) return;

        _currentIngredient = null;
    }
}
