using System.Collections.Generic;
using UnityEngine;

public class PlatingTile : ITemporalTile
{
    public Food Food => _food;
    public Item Item => _item;

    private Food _food;
    private Item _item;
    private List<Placeable> _placeables = new List<Placeable>();
    private PlatingTileUI _ui;

    public PlatingTile(PlatingTileUI ui)
    {
        _ui = ui;
    }

    public bool Accepts(Placeable placeable)
    {
        // TODO: add check for placing item in the current phase
        return (placeable is Food && SetupManager.Instance.CurrentPhase == GamePhase.Cooking) || (placeable is Item && _item == null);
    }

    public void Place(Placeable placeable)
    {
        if (placeable is Food newFood)
        {
            if (_food == null)
            {
                _ui.Place(newFood.UI);
                _food = newFood;
                _food.FoodUI.SetPlated(true);
                _placeables.Add(_food);
            }
            else
            {
                _placeables.Remove(_food);

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
                _placeables.Add(_food);
            }
        }
        else if (placeable is Item newItem)
        {
            _ui.Place(newItem.UI);
            _item = newItem;
            _placeables.Add(newItem);
        }
    }

    public Placeable Produces()
    {
        return _placeables.Count > 0 ? _placeables[_placeables.Count - 1] : null;
    }

    public void Remove()
    {
        if (_placeables.Count == 0) return;

        Placeable removed = _placeables[_placeables.Count - 1];

        if (removed is Food)
        {
            _food.FoodUI.SetPlated(false);
            _food = null;
        }
        else if (removed is Item)
        {
            _item = null;
        }

        _placeables.Remove(removed);
    }

    public void Remove(Food food)
    {
        if (_food == food)
        {
            _food.FoodUI.SetPlated(false);
            _food = null;
            _placeables.Remove(food);
        }
    }

    public void Remove(Item item)
    {
        if (_item == item)
        {
            _item = null;
            _placeables.Remove(item);
        }
    }

    public void Process(float dt)
    {
        _food?.Spoil(dt);
    }
}
