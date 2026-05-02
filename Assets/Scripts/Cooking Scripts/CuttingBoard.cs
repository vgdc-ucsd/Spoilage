using UnityEngine;
using UnityEngine.InputSystem;

public class CuttingBoard : CookingAppliance
{
    [Header("Cut Settings")]
    [SerializeField]
    private int _clicksPerState = 3;

    private IngredientObject _currentFood;

    private int _currentClicks;

    void Start()
    {
    }

    void Update ()
    {
        if (_currentFood == null) return;

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame) // F key
        {
        Cut();
        }
    }

    public override void OnPlaceFood(FoodGrab food)
    {
        _currentClicks = 0;
       _currentFood = food.GetComponent<IngredientObject>();

       if (_currentFood == null)
       {
           Debug.LogWarning("No IngredientObject found!");
           return;
       }


       Debug.Log("Food on cutting board");


       if (_currentFood.IngredientInstance.CurrentChoppedState == ChoppedState.Chopped)
       {
        Debug.Log("Food is already chopped");
        return;
       }

    }

    private void Cut()
    {
        //dont cut if alr chopped 
        if (_currentFood.IngredientInstance.CurrentChoppedState == ChoppedState.Chopped)
        {
            return;
        }
        
        _currentClicks++;
        Debug.Log("Chopping Food");

        if (_currentClicks >= _clicksPerState)
        {
            _currentFood.IngredientInstance.CurrentChoppedState = ChoppedState.Chopped;
            Debug.Log("Food is now chopped!");

            _currentClicks = 0; // reset for next use
        }
       
    }

    public override void OnRemoveFood()
    {
        _currentFood = null;
        _currentClicks = 0;
    }
    
}