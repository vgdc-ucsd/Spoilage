public abstract class Ingredient
{
    public enum IngredientState
    {
        Raw,
        Cooked,
        Burnt,
        Cut
    }

    private readonly IngredientData _data;

    private IngredientState _currentState;
    protected Ingredient(IngredientData data)
    {
        _data = data;
        _currentState = IngredientState.Raw;
    }

    public Ingredient(IngredientData data, IngredientState initialState)
    {
        _data = data;
        _currentState = initialState;
    }

    public IngredientData Data => _data;
    public IngredientState CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }
}

public sealed class Dough : Ingredient
{
    public Dough(IngredientData data) : base(data)
    {
    }
}
