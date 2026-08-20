using System.Collections.Generic;

public class AutomaticStation : Station
{
    private List<Food> _ingredients = new List<Food>();
    private Food _cookedFood;

    public override void Place(Placeable placeable)
    {
        if (placeable is not Food food) return;
        _ingredients.Add(food);
    }

    public override Placeable Produces()
    {
        if (SetupManager.Instance.CurrentPhase == GamePhase.Setup)
        {
            return this;
        }

        if (FoodState == FoodState.Preparing)
        {
            return null;
        }

        return _cookedFood;
    }

    public override void Remove()
    {
        _ingredients.Clear();
        FoodState = FoodState.Preparing;
    }

    public void Cook()
    {
        FoodState = FoodState.Prepared;
        _cookedFood = CookingManager.Instance.Process(_ingredients, this);
    }
}
