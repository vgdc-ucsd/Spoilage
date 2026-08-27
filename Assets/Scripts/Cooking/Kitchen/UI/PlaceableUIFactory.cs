using UnityEngine;

public class PlaceableUIFactory : Singleton<PlaceableUIFactory>
{
    [SerializeField] private FoodUI _foodUITemplate;
    [SerializeField] private AutomaticStationUI _automaticStationUITemplate;

    public FoodUI Generate(IngredientData ingredient, Transform parent)
    {
        FoodUI ui = Instantiate(_foodUITemplate, parent);
        ui.SetSprite(ingredient.NormalSprite);
        ui.SetBurnt(false);
        ui.ShowTimer(false);
        ui.transform.localPosition = Vector3.zero;
        if (ingredient.IsSmallIngredient) ui.transform.localScale = Vector3.one * 0.5f;
        return ui;
    }

    public PlaceableUI Generate(StationData station, Transform parent)
    {
        AutomaticStationUI ui = Instantiate(_automaticStationUITemplate, parent);
        ui.SetSprite(station.SpriteOff);
        ui.ShowTimer(false);
        ui.transform.localPosition = Vector3.zero;
        return ui;
    }
}
