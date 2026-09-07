public class ManualStation : Station
{
    public override PlaceableUI UI => _ui;
    public override StationUI StationUI => _ui;
    private ManualStationUI _ui;
    private int _clickCountdown;
    private const int NUM_CLICKS = 3;

    public ManualStation(StationData data, ManualStationUI ui)
    {
        _ui = ui;
        _ui.SetStation(this);
        Data = data;
        _clickCountdown = NUM_CLICKS;
    }

    public override void Place(Placeable placeable)
    {
        if (placeable is not Food food) return;

        ResetClicks();
        _ui.AddIngredient(food);
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
            _ui.ShowClicks(false);
        }
        else
        {
            _ingredients.Add(food);
            _ui.ShowClicks(true);
            if (_cookedFood != null) _ingredients.Add(_cookedFood);
            _cookedFood = null;
            FoodState = FoodState.Preparing;
        }    
    }

    public override void Remove()
    {
        base.Remove();
        _ui.ShowClicks(false);
    }

    public override void Cook()
    {
        base.Cook();
        _ui.ShowClicks(false);
    }

    public override void Process(float dt) { }

    private void ResetClicks()
    {
        _clickCountdown = NUM_CLICKS;
        _ui.SetClicks(0);
        _ui.ShowClicks(true);
    }

    public void Click()
    {
        if (Data.StationCategory == StationCategory.CuttingBoard)
        {
            SpoilageTriggerManager.Trigger(SpoilageCategory.DISTRESS);
        }

        if (FoodState == FoodState.Preparing)
        {    
            _clickCountdown--;
            float progress = (NUM_CLICKS - _clickCountdown) / (float)NUM_CLICKS;
            _ui.SetClicks(progress);
            if (_clickCountdown == 0) Cook();
        }
    }
}
