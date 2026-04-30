using UnityEngine;
using System.Collections.Generic;

public class KitchenTile : MonoBehaviour
{
    [Header("Tile Inventory")]
    public List<GameObject> objectsOnTile = new List<GameObject>();

    public bool CanPlaceObject(string type, GameObject movingObj = null)
    {
        // 1. If it's already here, it's fine (allows re-snapping)
        if (movingObj != null && objectsOnTile.Contains(movingObj)) return true;

        // 2. Anti-Stacking Check: Scan for existing appliances
        if (type == "Appliance")
        {
            foreach (var obj in objectsOnTile)
            {
                // If any object currently on the tile is an appliance, block placement
                if (obj.TryGetComponent(out ObjectGrab existingStation))
                {
                    return false;
                }
            }
        }

        // Rule: If empty, you can place an Appliance
        if (objectsOnTile.Count == 0)
            return type == "Appliance";

        // Rule: If there is 1 object (Appliance), you can place Food
        if (objectsOnTile.Count == 1)
        {
            bool isIngredient = movingObj != null && movingObj.GetComponent<IngredientObject>() != null;
            // Also adding "Plate" here just in case you use them later
            return type == "Food" || type == "Plate" || isIngredient;
        }

        return false;
    }

    public void PlaceObject(GameObject obj)
    {
        // Ensure the object is in the list
        if (!objectsOnTile.Contains(obj))
        {
            objectsOnTile.Add(obj);
        }

        // Force the physical snap so the tile "re-claims" it
        if (obj.TryGetComponent(out ObjectGrab station))
        {
            obj.transform.position = new Vector3(transform.position.x, transform.position.y, -2f);
            obj.transform.SetParent(null);
        }
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
