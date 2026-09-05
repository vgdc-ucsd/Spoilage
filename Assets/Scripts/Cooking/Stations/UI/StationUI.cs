using UnityEngine;

public class StationUI : PlaceableUI
{
    protected Station _station;
    protected static readonly Color32 s_normalColor = new Color32(22, 165, 31, 255);
    protected static readonly Color32 s_overcookColor = new Color32(103, 14, 14, 255);

    public virtual void SetStation(Station station)
    {
        _station = station;
    }

    public void BeginDrag()
    {
        _station.Produces()?.UI.gameObject.SetActive(true);
    }

    public void EndDrag()
    {
        _station.Produces()?.UI.gameObject.SetActive(false);
    }

    public void AddIngredient(Placeable placeable)
    {
        placeable.UI.gameObject.SetActive(false);
        SetSprite(_station.Data.SpriteOn);
    }

    public void Empty()
    {
        SetSprite(_station.Data.SpriteOff);
    }
}
