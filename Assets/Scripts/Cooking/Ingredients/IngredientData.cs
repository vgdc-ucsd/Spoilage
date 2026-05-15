using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Ingredients/IngredientData")]
public sealed class IngredientData : ScriptableObject
{
    [SerializeField] private string _name;

    [SerializeField] private float _spoilTime = 15f;
    [SerializeField] private float _qualityPercent;

    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _spoiledSprite;
    [SerializeField] private Sprite _platedSprite;

    public string Name => _name;
    public float SpoilTime => _spoilTime;
    public float QualityPercent {
        get => _qualityPercent; 
        set =>_qualityPercent = value;
    }
    public Sprite NormalSprite => _normalSprite;
    public Sprite SpoiledSprite => _spoiledSprite;
    public Sprite PlatedSprite => _platedSprite;
}