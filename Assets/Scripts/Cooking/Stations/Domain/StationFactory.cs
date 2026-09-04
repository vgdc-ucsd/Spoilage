using UnityEngine;

public class StationFactory : Singleton<StationFactory>
{
    public Station CreateStation(StationData data, Transform spawn)
    {
        Station station = null;
        PlaceableUI pui = PlaceableUIFactory.Instance.Generate(data, spawn);

        switch (data.StationType)
        {
            case StationType.Automatic:
                AutomaticStationUI asui = pui as AutomaticStationUI;
                station = new AutomaticStation(data, asui);
                break;
            case StationType.Manual:
                station = new ManualStation();
                break;
        }

        return station;   
    }    
}
