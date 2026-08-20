using System.Collections.Generic;
using UnityEngine;

public class KitchenTile : ITile
{
    private Placeable _current;
    private KitchenTileUI _ui;

    public KitchenTile(KitchenTileUI ui)
    {
        _ui = ui;
    }

    public bool Accepts(Placeable placeable)
    {
        return _current == null || (_current is Food or Station && placeable is Food && placeable != _current);
    }
    
    public void Place(Placeable placeable)
    {
        if (_current == null) 
        {
            _ui.Place(placeable.UI);
            _current = placeable;
        }
        else if (_current is Food currentFood && placeable is Food newFood)
        {
            List<Food> ingredients = new List<Food>
            {
                currentFood,
                newFood
            };

            Food food = CookingManager.Instance.Process(ingredients, null);
            Object.Destroy(currentFood.UI.gameObject);
            Object.Destroy(newFood.UI.gameObject);
            food.UI = PlaceableUIFactory.Instance.Generate(food.Data, _ui.transform);
            _ui.Place(food.UI);
            _current = food;
        }
        else if (_current is Station station && station.Accepts(placeable))
        {
            station.Place(placeable);
        }
    }

    public Placeable Produces()
    {
        if (_current is Station station)
        {
            if (SetupManager.Instance.CurrentPhase == GamePhase.Setup)
            {
                return station;
            }

            return station.Produces();
        }

        return _current;
    }
    
    public void Remove()
    {
        if (_current is Station station && SetupManager.Instance.CurrentPhase == GamePhase.Cooking)
        {
            station.Remove();
            return;
        }

        _current = null;
    }
}
