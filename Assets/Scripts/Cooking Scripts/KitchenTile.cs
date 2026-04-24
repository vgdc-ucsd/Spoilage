using UnityEngine;
using System.Collections.Generic;

public class KitchenTile : MonoBehaviour
{
    [Header("Tile Inventory")]
    public List<GameObject> objectsOnTile = new List<GameObject>();

    public bool CanPlaceObject(string type, GameObject movingObj = null)
    {
        if (movingObj != null && objectsOnTile.Contains(movingObj)) return true;

        foreach (GameObject obj in objectsOnTile)
        {
            // Block if an appliance is already here and we are dropping another appliance
            if (type == "Appliance" && obj.GetComponent<ObjectGrab>() != null) return false;

            // Block if food is already here and we are dropping more food
            if (type == "Food" && obj.GetComponent<FoodGrab>() != null) return false;
        }

        // Food needs at least one object (the appliance) to sit on
        if (type == "Food" && objectsOnTile.Count == 0) return false;

        return true;
    }

    // This is the function that caused the error. 
    // It now only takes ONE argument.
    public void PlaceObject(GameObject obj)
    {
        if (!objectsOnTile.Contains(obj))
        {
            objectsOnTile.Add(obj);
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
