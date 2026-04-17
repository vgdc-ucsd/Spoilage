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

    [SerializeField]
    private bool _isChopped;

    [SerializeField]
    private bool _needsChopping;

    [SerializeField]
    private Sprite _rawSprite;
    [SerializeField]
    private Sprite _cookedSprite;
    [SerializeField]
    private Sprite _burntSprite;
    [SerializeField]
    private Sprite unchoppedSprite;
    [SerializeField]
    private Sprite _choppedSprite;
    

    public string Name => _name;
    public bool NeedsCooking => _needsCooking;
    public int CookTime => _cookTime;
    public bool IsSpoiled => _isSpoiled;
    public Sprite RawSprite => _rawSprite;
    public Sprite CookedSprite => _cookedSprite;
    public Sprite BurntSprite => _burntSprite;
    public Sprite UnchoppedSprite => _unchoppedSprite;
    public Sprite ChoppedSprite => _choppedSprite;


}
