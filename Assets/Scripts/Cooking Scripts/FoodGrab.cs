using UnityEngine.InputSystem;
using UnityEngine;

public class FoodGrab : MonoBehaviour
{
    [SerializeField] private Transform _homeSpot;
    [SerializeField] private Transform _plateSpot;
    private CookingAppliance _activeAppliance;

    public bool TryGrab()
    {
        // Check if we are on a tile and remove from list if so
        KitchenTile tile = GetTileAtPosition(transform.position);
        if (tile != null)
        {
            if (tile.GetTopObject() != gameObject) return false;
            tile.RemoveObject(gameObject);
        }

        // Clean up stove reference
        if (_activeAppliance != null)
        {
            _activeAppliance.OnRemoveFood();
            _activeAppliance = null;
        }
        return true;
    }

    public void UpdateDragPosition()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        // Z -3 keeps food visually above appliances
        transform.position = new Vector3(mousePos.x, mousePos.y, -3f);
    }

    public void Drop()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.6f);

        // --- 1. PRIORITY PLATE CHECK (Independent of Tiles) ---
        foreach (var hit in hits)
        {
            if (hit.gameObject.name.Contains("Plate"))
            {
                IngredientObject info = GetComponent<IngredientObject>();
                // Only allow plating if the food is Cooked
                if (info != null && info.IngredientInstance.CurrentState == IngredientState.Cooked)
                {
                    // Snap to plate position (Plate can be anywhere)
                    transform.position = _plateSpot != null ? _plateSpot.position : hit.transform.position;

                    // Optional: If the plate IS on a tile, register it just in case
                    KitchenTile plateTile = hit.GetComponentInParent<KitchenTile>();
                    if (plateTile != null) plateTile.PlaceObject(gameObject, "Food");

                    Debug.Log("Food placed on Plate!");
                    return;
                }
            }
        }

        // --- 2. TILE-BASED LOGIC (For Stoves and Grid Counters) ---
        KitchenTile targetTile = null;
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out targetTile)) break;
        }

        if (targetTile != null && targetTile.CanPlaceObject("Food", gameObject))
        {
            foreach (var hit in hits)
            {
                // STOVE/POT CHECK
                if (hit.gameObject.name.Contains("StoveTop") || hit.gameObject.name.Contains("Pot"))
                {
                    CookingAppliance app = hit.GetComponentInParent<CookingAppliance>();
                    if (app != null)
                    {
                        targetTile.PlaceObject(gameObject, "Food");
                        _activeAppliance = app;
                        transform.position = hit.transform.position;
                        _activeAppliance.OnPlaceFood(this);
                        return;
                    }
                }
            }

            // Fallback: Drop on the counter tile itself
            targetTile.PlaceObject(gameObject, "Food");
            transform.position = targetTile.transform.position;
            return;
        }

        // --- 3. RETURN TO HOME (If no valid landing spot found) ---
        ReturnToHome();
    }

    private void ReturnToHome()
    {
        if (_homeSpot != null) transform.position = _homeSpot.position;
        _activeAppliance = null;
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
