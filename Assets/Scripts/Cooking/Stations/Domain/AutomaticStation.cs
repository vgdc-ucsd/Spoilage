using UnityEngine;

public class AutomaticStation : Station
{
    public override PlaceableUI UI => _ui;
    public override StationUI StationUI => _ui;
    private float _timer = 0f;
    private AutomaticStationUI _ui;
    private const float BASE_COOK_TIME = 5f;
    private Food _lastCookedFood;
    private float _currQualityBonus = 0f;

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
        
        float cookTime = HandleCookingSpeedUpgrades();
        _currQualityBonus += HandleQualityUpgrades();

        if (_timer >= cookTime)
        {
            if (!_overcook)
            {    
                Cook(_currQualityBonus);
                _lastCookedFood = _cookedFood;
                _overcook = true;
                _timer = 0f;
            }
            else
            {
                // Overcook TODO
            }
        }

        float progress = Mathf.Clamp01(_timer / cookTime);
        _ui.SetTimer(progress, _overcook);
    }

    public override void Place(Placeable placeable)
    {
        if (placeable is not Food food) return;

        _ui.AddIngredient(food);
        _timer = 0f;
        _overcook = false;

        if (Data.StationCategory == StationCategory.Grill || Data.StationCategory == StationCategory.Pot)
        {
            SpoilageTriggerManager.Trigger(SpoilageCategory.TEMPERATURE);
        }
        if (Data.StationCategory == StationCategory.Blender)
        {
            SpoilageTriggerManager.Trigger(SpoilageCategory.DISTRESS);
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

    public override void Cook(float bonusQuality)
    {
        base.Cook(bonusQuality);
        if (_slop) _ui.ShowTimer(false);
    }

    private float HandleCookingSpeedUpgrades()
    {
        switch (Data.StationCategory)
        {
            case StationCategory.Grill:
                if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.GrillSpeed))
                    return BASE_COOK_TIME * 0.75f;
                break;
            case StationCategory.Pot:
                if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.PotSpeed))
                    return BASE_COOK_TIME * 0.75f;
                break;
            case StationCategory.Oven:
                if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.OvenSpeed))
                    return BASE_COOK_TIME * 0.75f;
                break;
            default:
                break;
        }

        return BASE_COOK_TIME;
    }

    private float HandleQualityUpgrades()
    {
        switch (Data.StationCategory)
        {
            case StationCategory.Grill:
                if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.GrillQuality))
                {
                    if (CookingManager.Instance.Process(_ingredients, this, 0f) == _lastCookedFood) // if currently cooking food is same as last cooked food
                        return 5f;
                }
                break;
            default:
                break;
        }

        return 0f;
    }
}
