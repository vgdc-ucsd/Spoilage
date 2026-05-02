using UnityEngine;

public class ObjectGrab : MonoBehaviour
{
    public enum StationType { Appliance } // Removed Counter per your request
    public StationType type;

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
            // SUCCESS: Move to new tile
            newTile.PlaceObject(gameObject);
            currentTile = newTile;
        }
        else
        {
            // FAIL: Snap back to the last valid tile
            if (currentTile != null)
            {
                currentTile.PlaceObject(gameObject);
            }
            else
            {
                // Fallback if it somehow has no home (like starting in sidebar)
                // transform.position = _sidebarPosition; 
                transform.position = _sidebarPosition;
                Debug.Log("Invalid placement: Returning to sidebar.");
            }
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
