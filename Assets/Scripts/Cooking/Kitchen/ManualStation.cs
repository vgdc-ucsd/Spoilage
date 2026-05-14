using UnityEngine;

public class ManualStation : CookingStation
{
    [Header("Cut Settings")]
    [SerializeField] protected int _clicksPerState = 3;
    // TODO: create popup button that when clicked, calls OnAction()

    protected int _currentClicks;

    public override bool OnPlaceFood(FoodGrab food)
    {
        base.OnPlaceFood(food);

        // execute any special logic after calling base to ensure the food is set before accessing it

        _currentClicks = 0;
        Debug.Log($"Station updated. New item: {food.name}, Clicks reset to 0.");
        return true;
    }

    public override void OnRemoveFood()
    {
        // execute any special logic before calling base to ensure the food is still accessible if needed
        
        _currentClicks = 0;

        base.OnRemoveFood();
    }

    public virtual void OnAction()
    {
        _currentClicks++;
    }
}