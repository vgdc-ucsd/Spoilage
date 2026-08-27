using UnityEngine;

public class StationUnlockTileUI : TileUI
{
    private StationUnlockPopup _popup;

    public void Init(StationData stationData, StationUnlockPopup popup)
    {
        _popup = popup;
        Tile = new StationUnlockTile(stationData, this);
    }

    public void Hide()
    {
        _popup.Hide();
    }
}
