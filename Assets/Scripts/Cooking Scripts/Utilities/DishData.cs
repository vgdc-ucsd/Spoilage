using UnityEngine;

[CreateAssetMenu(fileName = "NewDish", menuName = "Utilities/DishData")]
public class DishData : ScriptableObject
{
    [SerializeField]
    private string _name;

    [SerializeField]
    private bool _isSpoiled;
    [SerializeField]
    private bool _isSeasoned;

    [SerializeField]
    private Sprite _normalSprite;
    [SerializeField]
    private Sprite _burntSprite;

    [SerializeField]
    private Sprite _seasonedSprite;
    [SerializeField]
    private Sprite _spoiledSprite;

    public string Name => _name;
    public bool IsSpoiled => _isSpoiled;
    public bool IsSeasoned => _isSeasoned;
    public Sprite NormalSprite => _normalSprite;
    public Sprite BurntSprite => _burntSprite;

    public Sprite SeasonedSprite => _seasonedSprite;
    public Sprite SpoiledSprite => _spoiledSprite;
}
