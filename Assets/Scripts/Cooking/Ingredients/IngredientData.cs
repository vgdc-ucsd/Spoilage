using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Ingredients/IngredientData")]
public sealed class IngredientData : ScriptableObject
{
    [SerializeField] private string _name;

    [SerializeField] private float _spoilTime;
    [SerializeField] private float _qualityPercent;

    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _spoiledSprite;

    public string Name => _name;
    public float SpoilTime => _spoilTime;
    public float QualityPercent {
        get => _qualityPercent; 
        set =>_qualityPercent = value;
    }
    public Sprite NormalSprite => _normalSprite;
    public Sprite SpoiledSprite => _spoiledSprite;
}