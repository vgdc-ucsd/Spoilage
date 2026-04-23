public abstract class Ingredient
{

    private readonly IngredientData _data;

    private IngredientState _currentState;
    protected Ingredient(IngredientData data)
    {
        _data = data;
        _data.IsSpoiled = false;
        _currentState = IngredientState.Raw;
    }

    public IngredientData Data => _data;
    public IngredientState CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }
}
public enum IngredientState
{
    Raw,
    Cooked,
    Burnt,
    Cut,
}
public sealed class Dough : Ingredient
{
    public Dough(IngredientData data) : base(data)
    {
    }
}
