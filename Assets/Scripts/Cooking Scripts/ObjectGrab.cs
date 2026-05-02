//using UnityEngine;

//public class ObjectGrab : MonoBehaviour
//{
//    public enum StationType { Counter, Appliance }
//    public StationType type;

//    private Vector3 _sidebarPosition;

//    private void Awake() => _sidebarPosition = transform.position;

//    public bool TryGrab()
//    {
//        // Find tile at current position
//        KitchenTile tile = GetTileAtPosition(transform.position);

//        if (tile != null)
//        {
//            // If it's not the top object, we can't grab it
//            if (tile.GetTopObject() != gameObject) return false;

//            tile.RemoveObject(gameObject);
//        }

//        if (type == StationType.Appliance) transform.SetParent(null);
//        return true;
//    }

//    public void UpdateDragPosition()
//    {
//        Vector2 mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
//        transform.position = new Vector3(mousePos.x, mousePos.y, (type == StationType.Appliance) ? -2f : -1f);
//    }

//    public void Drop()
//    {
//        KitchenTile tile = GetTileAtPosition(transform.position);
//        string typeString = (type == StationType.Counter) ? "Counter" : "Appliance";

//        if (tile != null && tile.CanPlaceObject(typeString, gameObject))
//        {
//            tile.PlaceObject(gameObject);

//            if (type == StationType.Appliance && tile.objectsOnTile.Count > 0)
//            {
//                GameObject counter = tile.objectsOnTile[0];
//                transform.position = counter.transform.position;
//                transform.SetParent(counter.transform);
//            }
//            else
//            {
//                transform.position = tile.transform.position;
//            }
//        }
//        else
//        {
//            transform.position = _sidebarPosition;
//            transform.SetParent(null);
//        }
//    }

//    private KitchenTile GetTileAtPosition(Vector2 pos)
//    {
//        Collider2D[] hits = Physics2D.OverlapPointAll(pos);
//        foreach (var hit in hits)
//        {
//            if (hit.TryGetComponent(out KitchenTile tile)) return tile;
//        }
//        return null;
//    }
//}
using UnityEngine;

public class ObjectGrab : MonoBehaviour
{
    public enum StationType { Counter, Appliance }
    public StationType type;
    private Vector3 _sidebarPosition;

    private void Awake()
    {
        // Store the starting position so we can snap back if a drop is invalid
        _sidebarPosition = transform.position;
    }

    public bool TryGrab()
    {
        if (LockLayout.IsLocked) return false;

        // 1. GLOBAL LOCK CHECK
        // If the "Start Day" button was pressed, this will be true and block all movement
        if (LockLayout.IsLocked) return false;

        // 2. OCCUPIED CHECK
        // Check if there is a FoodGrab component on this object or any of its children
        if (GetComponentInChildren<FoodGrab>() != null)
        {
            Debug.Log("Cannot move station: Food is currently on it!");
            return false;
        }

        // 3. TILE REGISTRY CLEANUP
        // If the stove was already on a tile, tell the tile it's leaving
        KitchenTile tile = GetTileAtPosition(transform.position);
        if (tile != null)
        {
            // Only allow grabbing if this is the top-most object (safety check)
            if (tile.GetTopObject() != gameObject) return false;
            tile.RemoveObject(gameObject);
        }

        transform.SetParent(null);
        return true;
    }

    public void UpdateDragPosition()
    {
        // Convert mouse screen position to world coordinates
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Z -2 keeps the stove visually "above" the floor (Z 0) but "below" food (Z -3)
        transform.position = new Vector3(mousePos.x, mousePos.y, -2f);
    }

    public void Drop()
    {
        KitchenTile tile = GetTileAtPosition(transform.position);

        // 4. SMART PLACEMENT CHECK
        // Ask the tile if it can accept an "Appliance"
        if (tile != null && tile.CanPlaceObject("Appliance", gameObject))
        {
            tile.PlaceObject(gameObject);
            transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, (type == StationType.Appliance) ? -2f : -1f);
        }
        else
        {
            // INVALID DROP: Return to the sidebar/original spot
            transform.position = _sidebarPosition;
            Debug.Log("Invalid placement: Returning to sidebar.");
        }
        transform.SetParent(null);
    }

    private KitchenTile GetTileAtPosition(Vector2 pos)
    {
        // Cast a point check to find the KitchenTile collider under the mouse
        Collider2D[] hits = Physics2D.OverlapPointAll(pos);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out KitchenTile tile))
            {
                return tile;
            }
        }
        return null;
    }
}
