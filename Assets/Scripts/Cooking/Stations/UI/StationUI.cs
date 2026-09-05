using System.Collections.Generic;
using UnityEngine;

public class StationUI : PlaceableUI
{
    protected Station _station;
    protected static readonly Color32 s_normalColor = new Color32(22, 165, 31, 255);
    protected static readonly Color32 s_overcookColor = new Color32(103, 14, 14, 255);

    public virtual void SetStation(Station station)
    {
        _station = station;
    }

    public void BeginDrag()
    {
        if (SetupManager.Instance.CurrentPhase == GamePhase.Cooking)
        {
            _station.Produces()?.UI.gameObject.SetActive(true);
        }
    }

    public void EndDrag()
    {
        if (SetupManager.Instance.CurrentPhase == GamePhase.Cooking)
        {
            _station.Produces()?.UI.gameObject.SetActive(_station.Data.Stack);
        }
    }

    public void AddIngredient(Placeable placeable)
    {
        SetSprite(_station.Data.SpriteOn);

        if (_station.Data.Stack)
        {
            placeable.UI.transform.SetParent(transform);
            placeable.UI.transform.localPosition = Vector3.zero;
            if (placeable.UI is FoodUI foodUI) foodUI.ShowTimer(false);
        }
        else
        {
            placeable.UI.gameObject.SetActive(false);
        }
    }

    public void Empty()
    {
        SetSprite(_station.Data.SpriteOff);
    }

    public void Cook(List<Food> ingredients, Food result)
    {
        foreach(Food ingredient in ingredients)
        {
            ingredient.UI.gameObject.SetActive(false);
        }

        result.UI.gameObject.SetActive(_station.Data.Stack);
    }
}
