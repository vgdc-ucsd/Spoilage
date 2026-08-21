using UnityEngine;

public class PlaceableUIFactory : Singleton<PlaceableUIFactory>
{
    [SerializeField] private PlaceableUI _placeableUITemplate;

    public PlaceableUI Generate(IngredientData ingredient, Transform parent)
    {
        PlaceableUI ui = Instantiate(_placeableUITemplate, parent);
        ui.SetSprite(ingredient.NormalSprite);
        ui.transform.localPosition = Vector3.zero;
        if (ingredient.IsSmallIngredient) ui.transform.localScale = Vector3.one * 0.5f;
        return ui;
    }

    public PlaceableUI Generate(StationData station, Transform parent)
    {
        PlaceableUI ui = Instantiate(_placeableUITemplate, parent);
        ui.SetSprite(station.SpriteOff);
        ui.transform.localPosition = Vector3.zero;
        return ui;
    }
}
