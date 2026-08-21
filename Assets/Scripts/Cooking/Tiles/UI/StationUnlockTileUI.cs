using UnityEngine;

public class StationUnlockTileUI : TileUI
{
    public void Init(StationData stationData)
    {
        Tile = new StationUnlockTile(stationData, this);
    }

    public void Hide()
    {
        
    }
}
