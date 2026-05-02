using UnityEngine;

public abstract class Dish 
{
    private readonly DishData _data;
     private DishState _currentState;
     
    protected Dish(DishData data)
    {
        _data = data;
        _currentState = DishState.Normal;
    }
    public DishState CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }
}
public enum DishState
{
    Normal,
    Spoiled,
    Burnt,
    Seasoned
}

public sealed class Flatbread : Dish
{
    public Flatbread(DishData data) : base(data)
    {
    }
}

