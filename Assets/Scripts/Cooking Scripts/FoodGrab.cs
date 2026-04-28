using UnityEngine.InputSystem;
using UnityEngine;

public class FoodGrab : MonoBehaviour
{
    [SerializeField] private Transform _homeSpot;
    [SerializeField] private Transform _plateSpot;
    private CookingAppliance _activeAppliance;
    private bool _isPlaced = false;

    public static bool CanMoveFood = true; // Default to true

    private void Awake()
    {
        // Check if there is a Start Day button in this specific scene
        WorldButton startButton = Object.FindFirstObjectByType<WorldButton>();

        if (startButton != null)
        {
            // If a button exists, lock the food until it's pressed
            CanMoveFood = false;
        }
        else
        {
            // If no button exists (like in a test scene), allow movement
            CanMoveFood = true;
        }
    }

    public bool TryGrab()
    {
        if (!CanMoveFood || _isPlaced) return false;

        // Check if we are on a tile and remove from list if so
        KitchenTile tile = GetTileAtPosition(transform.position);
        if (tile != null)
        {
            if (tile.GetTopObject() != gameObject) return false;
            tile.RemoveObject(gameObject);
        }

        // Clean up appliance reference if we pick it back up
        if (_activeAppliance != null)
        {
            _activeAppliance.OnRemoveFood();
            _activeAppliance = null;
        }

        return true;
    }

    private void OnMouseDown()
    {
        // Handled in TryGrab for the new logic
    }

    public void UpdateDragPosition()
    {
        if (!CanMoveFood || _isPlaced) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        // Z -3 keeps food visually above appliances
        transform.position = new Vector3(mousePos.x, mousePos.y, -3f);
    }

    public void Drop()
    {
        if (_isPlaced) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.6f);

        // --- 1. SCAN FOR PLATE OR APPLIANCE ---
        foreach (Collider2D hit in hits)
        {
            TrashCan trash = hit.GetComponentInParent<TrashCan>();

            if (trash != null)
            {
                trash.Trash(this);
                return;
            }

            Plate plate = hit.GetComponentInParent<Plate>() ?? hit.GetComponentInChildren<Plate>();

            if (plate != null || hit.gameObject.name.Contains("Plate"))
            {
                IngredientObject info = GetComponent<IngredientObject>();

                if (info != null && info.IngredientInstance != null)
                {
                    if (info.IngredientInstance.CurrentCookState == CookState.Cooked)
                    {
                        _activeAppliance = null;
                        if (plate != null) plate.AddIngredient(info);

                        transform.position = _plateSpot != null ? _plateSpot.position : hit.transform.position;
                        LockToPlate();
                        return;
                    }
                }
            }

            CookingAppliance app = hit.GetComponentInParent<CookingAppliance>();
            if (app != null)
            {
                _activeAppliance = app;
                transform.position = hit.transform.position;
                _activeAppliance.OnPlaceFood(this);

                KitchenTile tile = GetTileAtPosition(transform.position);
                if (tile != null) tile.PlaceObject(gameObject);
                return;
            }
        }

        // --- 2. TILE-BASED FALLBACK ---
        KitchenTile targetTile = GetTileAtPosition(transform.position);
        if (targetTile != null && targetTile.CanPlaceObject("Food", gameObject))
        {
            targetTile.PlaceObject(gameObject);
            transform.position = targetTile.transform.position;
            return;
        }

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

    public void LockToPlate()
    {
        _isPlaced = true;
    }
}
