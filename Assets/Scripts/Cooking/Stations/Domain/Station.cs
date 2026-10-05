using System.Collections.Generic;
using UnityEngine;

public enum FoodState
{
    Preparing,
    Prepared
}

public enum StationSound
{
    Start,
    Done,
    Remove,
    Click,
    Close
}

public abstract class Station : Placeable, ITemporalTile
{
    public abstract StationUI StationUI { get; }
    public StationData Data { get; protected set; }
    public FoodState FoodState { get; protected set; }

    protected List<Food> _ingredients = new List<Food>();
    protected Food _cookedFood;
    protected bool _overcook = false;
    protected bool _slop => _cookedFood != null && CookingManager.Instance.IsSlop(_cookedFood);

    public bool Accepts(Placeable placeable) { return placeable is Food food && food != _cookedFood; }
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

    protected void PlayStationSFX(StationSound sound)
    {
        string id = (Data.StationCategory, sound) switch
        {
            (StationCategory.Grill,            StationSound.Start)  => "Grill",
            (StationCategory.Pot,              StationSound.Start)  => "BoilingPot",
            (StationCategory.Pot,              StationSound.Done)   => "BoilingPotDone",
            (StationCategory.Blender,          StationSound.Start)  => "Blender",
            (StationCategory.Oven,             StationSound.Start)  => "ToasterOvenTurnOn",
            (StationCategory.Oven,             StationSound.Done)   => "ToasterOvenDone",
            (StationCategory.Oven,             StationSound.Remove) => "ToasterOvenOpen",
            (StationCategory.Oven,             StationSound.Close)  => "ToasterOvenClose",
            (StationCategory.CuttingBoard,     StationSound.Click)  => "CuttingBoard",
            (StationCategory.SeasoningStation, StationSound.Click)  => "Seasoning",
            _ => null
        };

        if (id != null) AudioManager.Instance.PlaySFX(id);
    }

    public virtual void Remove()
    {
        foreach (Food food in _ingredients)
        {
            food.Destroy();
        }

        _cookedFood = null;
        _ingredients.Clear();
        _overcook = false;
        FoodState = FoodState.Preparing;
    }

    public virtual void Cook(float bonusQuality)
    {
        FoodState = FoodState.Prepared;
        if (_cookedFood != null) _cookedFood.Destroy();
        _cookedFood = CookingManager.Instance.Process(_ingredients, this, bonusQuality);
        _cookedFood.SetUI(PlaceableUIFactory.Instance.Generate(_cookedFood.Data, UI.transform));
        StationUI.Cook(_ingredients, _cookedFood);

        if (_slop)
        {
            SpoilageTriggerManager.Trigger(SpoilageCategory.HUNGER);
        }
        else
        {
            SpoilageTriggerManager.Trigger(SpoilageCategory.DISGUST);
        }
    }
}
