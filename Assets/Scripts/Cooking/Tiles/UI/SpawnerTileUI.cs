using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnerTileUI : TileUI
{
    [SerializeField] private IngredientData _ingredient;
    [SerializeField] private Image _tileImage;
    private const float HOVER_DARKNESS = 0.9f;

    void Start()
    {
        Tile = new SpawnerTile(_ingredient, this);
    }

    public PlaceableUI GenerateFoodUI()
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

    public override void OnBeginDrag(PointerEventData _)
    {
        if (_locked) return;
        Tile.Produces()?.UI.gameObject.SetActive(true);
        base.OnBeginDrag(_);
    }

    public override void OnEndDrag(PointerEventData _)
    {
        if (_locked) return;
        Tile.Produces()?.UI.gameObject.SetActive(false);
        base.OnEndDrag(_);
    }
}