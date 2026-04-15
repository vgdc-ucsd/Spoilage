using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Ingredients/IngredientData")]
public sealed class IngredientData : ScriptableObject
{
    [SerializeField]
    private string _name;

    [SerializeField]
    private bool _needsCooking;

    [SerializeField]
    private int _cookTime;

    [SerializeField]
    private bool _isSpoiled;

    public enum IngredientState
    {
        Raw,
        Cooked,
        Burnt,
        Cut
    }

    [SerializeField]
    private IngredientState _state = IngredientState.Raw;
    public string Name => _name;
    public bool NeedsCooking => _needsCooking;
    public int CookTime => _cookTime;
    public bool IsSpoiled => _isSpoiled;


}
