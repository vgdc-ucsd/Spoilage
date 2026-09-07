using UnityEngine;

public class AutomaticStation : Station
{
    public override PlaceableUI UI => _ui;
    public override StationUI StationUI => _ui;
    private float _timer = 0f;
    private AutomaticStationUI _ui;
    private const float COOK_TIME = 5f;

    public AutomaticStation(StationData data, AutomaticStationUI ui)
    {
        _ui = ui;
        _ui.SetStation(this);
        Data = data;
    }

    public override void Process(float dt)
    {
        if (_ingredients.Count == 0 || _slop) return;

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
                // Overcook TODO
            }
        }

        float progress = Mathf.Clamp01(_timer / COOK_TIME);
        _ui.SetTimer(progress, _overcook);
    }

    public override void Place(Placeable placeable)
    {
        if (placeable is not Food food) return;

        _ui.AddIngredient(food);
        _timer = 0f;
        _overcook = false;

        if (_ingredients.Count == 0 && Data.Overcook)
        {
            SpoilageTriggerManager.Trigger(SpoilageCategory.TEMPERATURE);
        }

        if (_ingredients.Count == 0 
            && _cookedFood == null 
            && CookingManager.Instance.IsSlop(food)
        ) {
            FoodState = FoodState.Prepared;
            _cookedFood = food;
            _ui.ShowTimer(false);
        }
        else
        {
            _ingredients.Add(food);
            _ui.ShowTimer(true);
            if (_cookedFood != null) _ingredients.Add(_cookedFood);
            _cookedFood = null;
            FoodState = FoodState.Preparing;
        }    
    }

    public override void Remove()
    {
        base.Remove();
        _timer = 0;
        _ui.Empty();
        _ui.ShowTimer(false);
    }

    public override void Cook()
    {
        base.Cook();
        if (_slop) _ui.ShowTimer(false);
    }
}
