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
    public string Name => _name;
    public bool NeedsCooking => _needsCooking;
    public int CookTime => _cookTime;
    public bool IsSpoiled => _isSpoiled;


}
