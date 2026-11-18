using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Ingredients/IngredientData")]
public sealed class IngredientData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private float _spoilTime = 15f;
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _spoiledSprite;
    [SerializeField] private Sprite _plateSprite;
    [SerializeField] private bool _isSmallIngredient;
    [TextArea(5, 40)] [SerializeField] private string _recipeBookDescriptionUnspoiled;
    [TextArea(5, 40)] [SerializeField] private string _recipeBookDescriptionSpoiled;

    public string Name => _name;
    public float SpoilTime => _spoilTime;
    public Sprite NormalSprite => _normalSprite;
    public Sprite SpoiledSprite => _spoiledSprite;
    public Sprite PlateSprite => _plateSprite;
    public bool IsSmallIngredient => _isSmallIngredient;
    public string RecipeBookDescriptionUnspoiled => _recipeBookDescriptionUnspoiled;
    public string RecipeBookDescriptionSpoiled => _recipeBookDescriptionSpoiled;
}