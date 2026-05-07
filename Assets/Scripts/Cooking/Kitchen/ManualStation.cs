using UnityEngine;

public class ManualStation : CookingStation
{
    [Header("Cut Settings")]
    [SerializeField] protected int _clicksPerState = 3;
    [SerializeField] private IngredientTransform[] _transforms;
    // TODO: create popup button that when clicked, calls OnAction()

    protected int _currentClicks;

    public override void OnPlaceFood(FoodGrab food)
    {
        base.OnPlaceFood(food);

        // execute any special logic after calling base to ensure the food is set before accessing it

        _currentClicks = 0;
        Debug.Log($"Station updated. New item: {food.name}, Clicks reset to 0.");
    }

    public override void OnRemoveFood()
    {
        // execute any special logic before calling base to ensure the food is still accessible if needed
        
        _currentClicks = 0;

        base.OnRemoveFood();
    }

    public virtual void OnAction()
    {
        if (_currentFood == null)
        {
            Debug.LogWarning("No food on station.");
            return;
        }

        if (_currentFood.IngredientInstance == null || _currentFood.IngredientInstance.Data == null)
        {
            Debug.LogError("Food data is missing on " + _currentFood.name);
            return;
        }

        _currentClicks++;

        if (_currentClicks < _clicksPerState)
        {
            return;
        }

        IngredientData currentData = _currentFood.IngredientInstance.Data;

        if (!TryGetTransform(currentData, out IngredientTransform transform))
        {
            Debug.Log($"{gameObject.name} cannot process {currentData.Name}");
            _currentClicks = 0;
            return;
        }

        if (transform.output == null)
        {
            Debug.LogError("Output ingredient is missing on " + gameObject.name);
            _currentClicks = 0;
            return;
        }

        _currentFood.ChangeIngredient(transform.output);
        Debug.Log($"Manual action finished! {currentData.Name} is now {transform.output.Name}!");

        _currentClicks = 0;
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