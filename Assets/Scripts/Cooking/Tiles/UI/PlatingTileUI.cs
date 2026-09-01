using UnityEngine;

public class PlatingTileUI : TileUI
{
    private PlatingTile _tile;
    public PlatingTile PlatingTile => _tile;

    public void Init()
    {
        _tile = new PlatingTile(this);
        Tile = _tile;
    }

    public void Place(PlaceableUI ui)
    {
        ui.gameObject.SetActive(true);
        ui.transform.SetParent(transform);
        ui.transform.localPosition = Vector3.zero;
    }
}
