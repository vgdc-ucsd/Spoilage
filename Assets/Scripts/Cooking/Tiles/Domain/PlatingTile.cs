using System.Collections.Generic;
using UnityEngine;

public class PlatingTile : ITemporalTile
{
    public Food Food => _food;

    private Food _food;
    private PlatingTileUI _ui;

    public PlatingTile(PlatingTileUI ui)
    {
        _ui = ui;
    }

    public bool Accepts(Placeable placeable)
    {
        return placeable is Food;
    }
    
    public void Place(Placeable placeable)
    {
        Food newFood = placeable as Food;

        if (_food == null) 
        {
            _ui.Place(newFood.UI);
            _food = newFood;
            _food.FoodUI.SetPlated(true);
        }
        else
        {
            List<Food> ingredients = new List<Food>
            {
                _food,
                newFood
            };

            Food food = CookingManager.Instance.Process(ingredients, null);
            Object.Destroy(_food.UI.gameObject);
            Object.Destroy(newFood.UI.gameObject);
            food.SetUI(PlaceableUIFactory.Instance.Generate(food.Data, _ui.transform));
            _ui.Place(food.UI);
            _food = food;
            _food.FoodUI.SetPlated(true);
        }
    }

    public Placeable Produces()
    {
        return _food;
    }
    
    public void Remove()
    {
        _food.FoodUI.SetPlated(false);
        _food = null;
    }

    public void Process(float dt)
    {
        _food?.Spoil(dt);
    }
}
