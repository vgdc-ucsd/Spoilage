public abstract class Ingredient
{

    private readonly IngredientData _data;

    private CookState _currentCookState;
    private ChoppedState _currentChoppedState;
    private bool _isSpoiled;
    /*
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
    */
    protected Ingredient(IngredientData data)
    {
        _data = data;
        _currentCookState = CookState.Raw;
        _currentChoppedState = ChoppedState.Unchopped;
    }

    public IngredientData Data
    {
        get { return _data; }
    }

    public CookState CurrentCookState
    {
        get { return _currentCookState; }
        set { _currentCookState = value; }
    }

    public ChoppedState CurrentChoppedState
    {
        get { return _currentChoppedState; }
        set { _currentChoppedState = value; }
    }

    public bool IsSpoiled
    {
        get => _isSpoiled;
        set => _isSpoiled = value;
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
