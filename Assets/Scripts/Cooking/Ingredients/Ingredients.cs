public abstract class Ingredient
{

    private readonly IngredientData _data;

    private CookState _currentCookState;
    private ChoppedState _currentChoppedState;
    private bool _isSpoiled;
    
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
    Toasted,
    Grilled,
    Boiled,
    Burnt
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

public sealed class Cheese : Ingredient
{
    public Cheese(IngredientData data) : base(data)
    {
    }
}

public sealed class Meat : Ingredient
{
    public Meat(IngredientData data) : base(data)
    {
    }
}

public sealed class Vegetable : Ingredient
{
    public Vegetable(IngredientData data) : base(data)
    {
    }
}

public sealed class GenericIngredient : Ingredient
{
    public GenericIngredient(IngredientData data) : base(data)
    {
    }
}
