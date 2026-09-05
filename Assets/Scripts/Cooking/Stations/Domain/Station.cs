using System.Collections.Generic;
using UnityEngine;

public enum FoodState
{
    Preparing,
    Prepared
}

public abstract class Station : Placeable, ITemporalTile
{
    public abstract StationUI StationUI { get; }
    public StationData Data { get; protected set; }
    public FoodState FoodState { get; protected set; }

    protected List<Food> _ingredients = new List<Food>();
    protected Food _cookedFood;
    protected bool _overcook = false;
    protected bool _justSlop => _ingredients.Count == 1 && CookingManager.Instance.IsSlop(_ingredients[0]);

    public bool Accepts(Placeable placeable) { return placeable is Food; }
    public abstract void Process(float dt);
    public abstract void Place(Placeable placeable);
    
    public Placeable Produces()
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

    public virtual void Remove()
    {
        if (!_justSlop)
        {    
            foreach (Food food in _ingredients)
            {
                food.Destroy();
            }
        }

        _cookedFood = null;
        _ingredients.Clear();
        _overcook = false;
        FoodState = FoodState.Preparing;
    }

    public void Cook()
    {
        FoodState = FoodState.Prepared;
        if (_cookedFood != null) _cookedFood.Destroy();
        _cookedFood = CookingManager.Instance.Process(_ingredients, this);
        _cookedFood.SetUI(PlaceableUIFactory.Instance.Generate(_cookedFood.Data, UI.transform));
        StationUI.Cook(_ingredients, _cookedFood);
    }

    public virtual void MakeSlop()
    {
        Food slop = CookingManager.Instance.CreateSlop(UI.transform);
        _cookedFood = slop;
        _ingredients.Clear();
        _ingredients.Add(slop);
    }
}
