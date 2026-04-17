public abstract class Ingredient
{

    private readonly IngredientData _data;

    private CookState _currentCookState;
    private ChoppedState _currentChoppedState;
    protected Ingredient(IngredientData data)
    {
        _data = data;
        _cookState = _cookState.Raw;
        _choppedState = _choppedState.Unchopped;
    }

    public IngredientData Data => _data;
    public IngredientState CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }
}
public enum CookState
{
    Raw,
    Cooked,
    Burnt,
}

public enum ChoppedState
{
    Chopped,
    Unchopped,
}

public 
public sealed class Dough : Ingredient
{
    public Dough(IngredientData data) : base(data)
    {
    }
}

public sealed class Vegetable : Ingredient
{
    public Vegetable(IngredientData data) : base(data)
    {
    }
}
