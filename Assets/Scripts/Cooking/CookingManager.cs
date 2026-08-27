using System.Collections.Generic;
using UnityEngine;

public class CookingManager : Singleton<CookingManager>
{
    [SerializeField] private IngredientData _slopData;
    private List<ITemporalTile> _tiles = new List<ITemporalTile>();

    public Food Process(List<Food> ingredients, Station station)
    {
        IngredientData data = RecipeManager.Instance.LookupResult(ingredients, station);
        if (data == _slopData) return new Food(_slopData, 0f, 1f);

        float quality = 0f;
        float spoilage = 0f;
        // Seasoning?

        foreach (Food food in ingredients)
        {
            quality += food.QualityPercent;
            spoilage += food.SpoilagePercent;
        }

        quality /= ingredients.Count;
        spoilage /= ingredients.Count;
        
        return new Food(data, quality, spoilage);
    }

    public Food CreateSlop(Transform uiTransform)
    {
        Food slop = new Food(_slopData, 0f, 1f);
        slop.SetUI(PlaceableUIFactory.Instance.Generate(_slopData, uiTransform));
        slop.UI.gameObject.SetActive(false);
        return slop;
    }

    public bool IsSlop(Food food)
    {
        return food.Data == _slopData;
    }

    public void SetTiles(List<ITemporalTile> tiles)
    {
        _tiles = tiles;
    }

    public void Update()
    {
        foreach (ITemporalTile tile in _tiles)
        {
            tile.Process(Time.deltaTime);
        }
    }
}
