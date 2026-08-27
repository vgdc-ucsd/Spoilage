using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnerTileUI : TileUI
{
    [SerializeField] private IngredientData _ingredient;
    [SerializeField] private Image _tileImage;
    private const float HOVER_DARKNESS = 0.9f;

    public void Init()
    {
        Tile = new SpawnerTile(_ingredient, this);        
    }

    public FoodUI GenerateFoodUI()
    {
        return PlaceableUIFactory.Instance.Generate(_ingredient, transform);
    }

    public override void OnPointerEnter(PointerEventData _)
    {
        if (_locked) return;
        _tileImage.color = Color.white;
        base.OnPointerEnter(_);
    }
    
    public override void OnPointerExit(PointerEventData _)
    {
        if (_locked) return;
        _tileImage.color = Color.white * HOVER_DARKNESS;
        base.OnPointerExit(_);
    }
}