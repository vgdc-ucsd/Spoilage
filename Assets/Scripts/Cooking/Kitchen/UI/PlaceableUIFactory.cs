using UnityEngine;

public class PlaceableUIFactory : Singleton<PlaceableUIFactory>
{
    [SerializeField] private FoodUI _foodUITemplate;
    [SerializeField] private AutomaticStationUI _automaticStationUITemplate;
    [SerializeField] private ManualStationUI _manualStationUITemplate;

    public FoodUI Generate(IngredientData ingredient, Transform parent)
    {
        FoodUI ui = Instantiate(_foodUITemplate, parent);
        ui.SetSprite(ingredient.NormalSprite);
        ui.SetBurnt(false);
        ui.ShowTimer(false);
        ui.SetPlated(false);
        ui.transform.localPosition = Vector3.zero;
        return ui;
    }

    public PlaceableUI Generate(StationData station, Transform parent)
    {
        switch (station.StationType)
        {
            case StationType.Automatic:
                return GenerateAutomaticStation(station, parent);
            case StationType.Manual:
                return GenerateManualStation(station, parent);
            default:
                return null;
        }
    }

    private AutomaticStationUI GenerateAutomaticStation(StationData station, Transform parent)
    {
        AutomaticStationUI ui = Instantiate(_automaticStationUITemplate, parent);
        ui.SetSprite(station.SpriteOff);
        ui.ShowTimer(false);
        ui.transform.localPosition = Vector3.zero;
        return ui;
    }

    private ManualStationUI GenerateManualStation(StationData station, Transform parent)
    {
        ManualStationUI ui = Instantiate(_manualStationUITemplate, parent);
        ui.SetSprite(station.SpriteOff);
        ui.ShowClicks(false);
        ui.transform.localPosition = Vector3.zero;
        return ui;
    }
}
