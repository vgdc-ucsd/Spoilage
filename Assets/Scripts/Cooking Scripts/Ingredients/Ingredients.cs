public abstract class Ingredient
{

    private readonly IngredientData _data;

    private IngredientState _currentState;
    private bool _isSpoiled;
    protected Ingredient(IngredientData data)
    {
        _data = data;
        _isSpoiled = false;
        _currentState = IngredientState.Raw;
    }

    public IngredientData Data => _data;
    public IngredientState CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }

    public bool IsSpoiled
    {
        get => _isSpoiled;
        set => _isSpoiled = value;
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
