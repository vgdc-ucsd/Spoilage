using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Ingredients/IngredientData")]
public sealed class IngredientData : ScriptableObject
{
    [SerializeField] private string _name;

    [SerializeField] private float _spoilTime = 15f;

    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _spoiledSprite;

    public string Name => _name;
    public float SpoilTime => _spoilTime;
    public Sprite NormalSprite => _normalSprite;
    public Sprite SpoiledSprite => _spoiledSprite;
}