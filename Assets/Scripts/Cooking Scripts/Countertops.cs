using UnityEngine;

public class Countertops : MonoBehaviour
{
    [SerializeField] private Timer _timer;
    private IngredientObject _currentFood;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlaceFood(FoodGrab food)
    {
        _currentFood = food.GetComponent<IngredientObject>();
        IngredientBehaviour ingredientBehaviour = food.GetComponent<IngredientBehaviour>();

        if (_currentFood == null || ingredientBehaviour == null)
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

        if (_currentFood.IngredientInstance == null || _currentFood.IngredientInstance.Data == null)
        {
            Debug.LogError("Food data is missing on " + _currentFood.name);
            return;
        }

        ingredientBehaviour.PutOnSpoilSurface();

    }


    public void OnRemoveFood()
    {
        if (_currentFood == null)
        {
            return;
        }

        IngredientBehaviour ingredientBehaviour = _currentFood.GetComponent<IngredientBehaviour>();

        if (ingredientBehaviour != null)
        {
            ingredientBehaviour.RemoveFromSpoilSurface();
        }
        Debug.Log("Food removed from counter");
        _currentFood = null;
    }

}
