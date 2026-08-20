using UnityEngine;

public class PlaceableUIFactory : Singleton<PlaceableUIFactory>
{
    [SerializeField] private PlaceableUI _placeableUITemplate;

    public PlaceableUI Generate(IngredientData ingredient, Transform parent)
    {
        PlaceableUI ui = Instantiate(_placeableUITemplate, parent);
        ui.SetSprite(ingredient.NormalSprite);
        ui.transform.localPosition = Vector3.zero;
        return ui;
    }
}
