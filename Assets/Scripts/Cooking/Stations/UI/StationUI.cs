public class StationUI : PlaceableUI
{
    protected Station _station;

    public void SetStation(Station station)
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
}
