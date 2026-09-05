public class ManualStation : Station
{
    public override PlaceableUI UI => _ui;
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

        _ingredients.Add(food);
        _ui.AddIngredient(placeable);
        
        if (_justSlop)
        {
            FoodState = FoodState.Prepared;
            _cookedFood = food;
        }
        else
        {
            _ui.ShowClicks(true);
            FoodState = FoodState.Preparing;
        }

        if (_overcook) Cook();
    }

    public override void Remove()
    {
        base.Remove();
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
        if (FoodState == FoodState.Preparing)
        {    
            _clickCountdown--;
            float progress = (NUM_CLICKS - _clickCountdown) / (float)NUM_CLICKS;
            _ui.SetClicks(progress);
            if (_clickCountdown == 0) Cook();
        }
    }
}
