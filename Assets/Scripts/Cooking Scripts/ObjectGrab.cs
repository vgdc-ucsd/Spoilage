using UnityEngine;

public class ObjectGrab : MonoBehaviour
{
    public enum StationType { Appliance } // Removed Counter per your request
    public StationType type;

    // Track the tile this appliance is currently assigned to
    public KitchenTile currentTile;

    public bool TryGrab()
    {
        if (LockLayout.IsLocked) return false;

        // Use our tracked currentTile instead of searching by position
        if (currentTile != null)
        {
            // Only allow grab if this is the top object (or handle as needed)
            if (currentTile.GetTopObject() != gameObject) return false;

            currentTile.RemoveObject(gameObject);
        }

        transform.SetParent(null);
        return true;
    }

    public void UpdateDragPosition()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        // Force Z to -2f for dragging appliances
        transform.position = new Vector3(mousePos.x, mousePos.y, -2f);
    }

    public void Drop()
    {
        KitchenTile newTile = GetTileAtPosition(transform.position);

        // Check if the new location is valid
        if (newTile != null && newTile.CanPlaceObject("Appliance", gameObject))
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
            }
        }
        transform.SetParent(null);
    }

    private KitchenTile GetTileAtPosition(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(pos);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out KitchenTile tile)) return tile;
        }
        return null;
    }
}
