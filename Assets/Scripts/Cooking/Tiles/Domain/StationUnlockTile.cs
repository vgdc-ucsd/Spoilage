
using UnityEngine;

public class StationUnlockTile : ITile
{
    private StationData _stationData;
    private Station _station;
    private StationUnlockTileUI _ui;

    public StationUnlockTile(StationData data, StationUnlockTileUI ui)
    {
        _stationData = data;
        _ui = ui;
        
        switch (data.StationType)
        {
            case StationType.Automatic:
                _station = new AutomaticStation();
                break;
            case StationType.Manual:
                _station = new ManualStation();
                break;
            default:
                Debug.LogError("Station type not recognized");
                break;
        }
        
        _station.UI = PlaceableUIFactory.Instance.Generate(data, _ui.transform);
    }

    public bool Accepts(Placeable _) { return false; }
    public void Place(Placeable _) { }

    public Placeable Produces()
    {
        return _station;
    }

    public void Remove()
    {
        _station = null;
        _ui.Hide();
    }
}
