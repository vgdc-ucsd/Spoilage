using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Ingredients/IngredientData")]
public sealed class IngredientData : ScriptableObject
{
    [SerializeField]
    private string _name;

    [SerializeField]
    private int _cookTime;

    [SerializeField]
    private int _spoilTime;

    [SerializeField]
    private bool _isSpoiled;

    [SerializeField]
    private Sprite _rawSprite;
    [SerializeField]
    private Sprite _cookedSprite;
    [SerializeField]
    private Sprite _burntSprite;
    [SerializeField]
    private Sprite _choppedSprite;


    public string Name => _name;
    public int CookTime => _cookTime;
    public int SpoilTime => _spoilTime;
    public bool IsSpoiled => _isSpoiled;
    public Sprite RawSprite => _rawSprite;
    public Sprite CookedSprite => _cookedSprite;
    public Sprite BurntSprite => _burntSprite;
    public Sprite ChoppedSprite => _choppedSprite;


}
