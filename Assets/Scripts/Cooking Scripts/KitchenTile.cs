//using UnityEngine;
//using System.Collections.Generic;

//public class KitchenTile : MonoBehaviour
//{
//    [Header("Tile Inventory")]
//    public List<GameObject> objectsOnTile = new List<GameObject>();

//    public bool CanPlaceObject(string type, GameObject movingObj = null)
//    {
//        // 1. If it's already in the list (like when picking up/dropping back), allow it
//        if (movingObj != null && objectsOnTile.Contains(movingObj)) return true;

//        // 2. Standard Stack Rules
//        if (objectsOnTile.Count == 0)
//        {
//            return type == "Counter";
//        }

//        if (objectsOnTile.Count == 1)
//        {
//            // If there's 1 item, it MUST be a Counter to allow an Appliance on top
//            ObjectGrab firstObj = objectsOnTile[0].GetComponent<ObjectGrab>();
//            if (firstObj != null && firstObj.type == ObjectGrab.StationType.Counter)
//            {
//                return type == "Appliance";
//            }
//        }

//        // 3. If tile has 2 items, it is full
//        return false;
//    }

//    public void PlaceObject(GameObject obj, string type = "")
//    {
//        if (!objectsOnTile.Contains(obj))
//        {
//            objectsOnTile.Add(obj);
//        }
//    }

//    public void RemoveObject(GameObject obj)
//    {
//        if (objectsOnTile.Contains(obj))
//        {
//            objectsOnTile.Remove(obj);
//        }
//    }

//    public GameObject GetTopObject()
//    {
//        if (objectsOnTile.Count > 0)
//            return objectsOnTile[objectsOnTile.Count - 1];
//        return null;
//    }
//}
using UnityEngine;
using System.Collections.Generic;

public class KitchenTile : MonoBehaviour
{
    [Header("Tile Inventory")]
    public List<GameObject> objectsOnTile = new List<GameObject>();

    public bool CanPlaceObject(string type, GameObject movingObj = null)
    {
        if (movingObj != null && objectsOnTile.Contains(movingObj)) return true;

        // Standard Stack Rules
        if (objectsOnTile.Count == 0) return type == "Counter";

        if (objectsOnTile.Count == 1)
        {
            if (objectsOnTile[0].TryGetComponent(out ObjectGrab counter) && counter.type == ObjectGrab.StationType.Counter)
                return type == "Appliance";
        }

        // Allow placement if type is "Food" OR if the object has an IngredientObject component
        if (objectsOnTile.Count == 2)
        {
            bool isIngredient = movingObj != null && movingObj.GetComponent<IngredientObject>() != null;
            return type == "Food" || isIngredient;
        }

        return false;
    }

    public void PlaceObject(GameObject obj, string type = "")
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
