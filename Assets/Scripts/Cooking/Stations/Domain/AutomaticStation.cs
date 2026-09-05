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

    public override void Remove()
    {
        base.Remove();
        _timer = 0;
        _ui.Empty();
        _ui.ShowTimer(false);
    }

    public override void MakeSlop()
    {
        base.MakeSlop();
        _ui.ShowTimer(false);
    }
}
