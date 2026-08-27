using System.Collections.Generic;
using UnityEngine;

public class AutomaticStation : Station
{
    public override PlaceableUI UI => _ui;

    private List<Food> _ingredients = new List<Food>();
    private Food _cookedFood;
    private float _timer = 0f;
    private AutomaticStationUI _ui;
    private bool _overcook = false;
    private bool _justSlop => _ingredients.Count == 1 && CookingManager.Instance.IsSlop(_ingredients[0]);

    private const float COOK_TIME = 5f;

    public AutomaticStation(StationData data, AutomaticStationUI ui)
    {
        _ui = ui;
        _ui.SetStation(this);
        Data = data;
    }

    public override void Process(float dt)
    {
        if (_ingredients.Count == 0 || _justSlop) return;

        _timer += dt;
        
        if (_timer >= COOK_TIME)
        {
            if (!_overcook)
            {    
                Cook();
                _overcook = true;
                _timer = 0f;
            }
            else
            {
                MakeSlop();
            }
        }

        float progress = Mathf.Clamp01(_timer / COOK_TIME);
        _ui.SetTimer(progress, _overcook);
    }

    public override void Place(Placeable placeable)
    {
        if (placeable is not Food food) return;

        if (_ingredients.Count == 0 && Data.Overcook)
        {
            SpoilageTriggerManager.Trigger(SpoilageCategory.TEMPERATURE);
        }

        _ingredients.Add(food);
        _ui.AddIngredient(placeable);
        _timer = 0f;
        
        if (_justSlop)
        {
            FoodState = FoodState.Prepared;
            _cookedFood = food;
        }
        else
        {
            _ui.ShowTimer(true);
            FoodState = FoodState.Preparing;
        }

        if (_overcook) Cook();
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
        if (!_justSlop)
        {    
            foreach (Food food in _ingredients)
            {
                food.Destroy();
            }
        }

        _cookedFood = null;
        _ingredients.Clear();
        _timer = 0;
        _overcook = false;
        FoodState = FoodState.Preparing;
        _ui.Empty();
        _ui.ShowTimer(false);
    }

    public void Cook()
    {
        FoodState = FoodState.Prepared;
        if (_cookedFood != null) _cookedFood.Destroy();
        _cookedFood = CookingManager.Instance.Process(_ingredients, this);
        _cookedFood.SetUI(PlaceableUIFactory.Instance.Generate(_cookedFood.Data, _ui.transform));
        _cookedFood.UI.gameObject.SetActive(false);
    }

    public void MakeSlop()
    {
        Food slop = CookingManager.Instance.CreateSlop(_ui.transform);
        _cookedFood = slop;
        _ingredients.Clear();
        _ingredients.Add(slop);
        _ui.ShowTimer(false);
    }
}
