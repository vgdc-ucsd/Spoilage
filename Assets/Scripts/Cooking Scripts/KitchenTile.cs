using UnityEngine;
using System.Collections.Generic;

public class KitchenTile : MonoBehaviour
{
    [Header("Tile Inventory")]
    public List<GameObject> objectsOnTile = new List<GameObject>();

    public bool CanPlaceObject(string type, GameObject movingObj = null)
    {
        if (movingObj != null && objectsOnTile.Contains(movingObj)) return true;

        // Rule: If empty, you can place a Counter OR an Appliance
        if (objectsOnTile.Count == 0)
            return type == "Appliance";

        // Rule: If there is 1 object (Counter or Appliance), you can place Food
        if (objectsOnTile.Count == 1)
        {
            bool isIngredient = movingObj != null && movingObj.GetComponent<IngredientObject>() != null;
            return type == "Food" || isIngredient;
        }

        return false;
    }

    public void PlaceObject(GameObject obj)
    {
        if (!objectsOnTile.Contains(obj)) objectsOnTile.Add(obj);
    }

    public void RemoveObject(GameObject obj)
    {
        if (objectsOnTile.Contains(obj)) objectsOnTile.Remove(obj);
    }

    public GameObject GetTopObject()
    {
        return objectsOnTile.Count > 0 ? objectsOnTile[objectsOnTile.Count - 1] : null;
    }
}
