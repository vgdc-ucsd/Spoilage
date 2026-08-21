using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KitchenTileUI : TileUI
{
    [SerializeField] private Image _tileImage;
    private const float HOVER_DARKNESS = 0.2f;
    private const float LOCKED_DARKNESS = 0.5f;

    void Start()
    {
        Tile = new KitchenTile(this);
    }

    public void Place(PlaceableUI ui)
    {
        ui.gameObject.SetActive(true);
        ui.transform.SetParent(transform);
        ui.transform.localPosition = Vector3.zero;
    }

    public override void Lock(bool locked)
    {
        if (locked) _tileImage.color = Color.white * LOCKED_DARKNESS;
        else _tileImage.color = Color.clear;
        base.Lock(locked);
    }

    public override void OnPointerEnter(PointerEventData _)
    {
        if (_locked) return;
        _tileImage.color = Color.white * HOVER_DARKNESS;
        base.OnPointerEnter(_);
    }
    
    public override void OnPointerExit(PointerEventData _)
    {
        if (_locked) return;
        _tileImage.color = Color.clear;
        base.OnPointerExit(_);
    }
}
