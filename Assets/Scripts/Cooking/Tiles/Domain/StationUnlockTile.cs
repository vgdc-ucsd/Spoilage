public class StationUnlockTile : ITile
{
    private Station _station;
    private StationUnlockTileUI _ui;

    public StationUnlockTile(StationData data, StationUnlockTileUI ui)
    {
        _ui = ui;
        _station = StationFactory.Instance.CreateStation(data, ui.transform);
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
        SetupManager.Instance.SetAllStationsPlaced();
    }
}
