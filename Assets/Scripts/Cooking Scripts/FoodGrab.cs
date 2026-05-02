using UnityEngine.InputSystem;
using UnityEngine;

public class FoodGrab : MonoBehaviour
{
    private bool _hasHomePosition;
    private bool _isPlaced = false;
    private bool _cameFromFridge = true;
    private Vector3 _homePosition;
    private Vector3 _returnPosition;
    [SerializeField] private Transform _plateSpot;
    private FoodSpawner _spawner;
    private CookingAppliance _activeAppliance;
    private CookingAppliance _returnAppliance;


    public static bool CanMoveFood = true; // Default to true

    private void Awake()
    {
        // Check if there is a Start Day button in this specific scene
        WorldButton startButton = FindAnyObjectByType<WorldButton>();

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

    public void SetHomePosition(Vector3 position)
    {
        _homePosition = position;
        _hasHomePosition = true;
    }

    public void SetCameFromFridge(bool value)
    {
        _cameFromFridge = value;
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
            _returnAppliance = _activeAppliance;
            _returnPosition = _activeAppliance.transform.position;

            _activeAppliance.OnRemoveFood();
            _activeAppliance = null;
        }
        else
        {
            _returnAppliance = null;
        }   


        if (_spawner != null)
        {
            _spawner.SpawnFood();
            _spawner = null;
        }

        return true;
    }

    public void SetSpawner(FoodSpawner spawner)
    {
        _spawner = spawner;
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

        // INCREASE the radius to 1.0f to make it easier to hit the plate
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.0f);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("Food dropped over: " + hit.gameObject.name);

            // --- 1. SCAN FOR PLATE OR TRASH OR APPLIANCE ---
            TrashCan trash = hit.GetComponentInParent<TrashCan>();
            if (trash != null)
            {
                trash.Trash(this);
                return;
            }

            // This looks for the Plate script anywhere on the object we hit or its parents
            Plate plate = hit.GetComponentInParent<Plate>() ?? hit.GetComponentInChildren<Plate>();

            if (plate != null)
            {
                Debug.Log("!!! PLATE DETECTED !!!"); // Look for this in the console!
                IngredientObject info = GetComponent<IngredientObject>();

                if (info != null)
                {
                    plate.AddIngredient(info);

                    transform.position = _plateSpot != null ? _plateSpot.position : hit.transform.position;
                    LockToPlate();
                    plate.PrintIngredients();
                    return;
                }
            }

            CookingAppliance app = hit.GetComponentInParent<CookingAppliance>();
            if (app != null)
            {
                _activeAppliance = app;
                transform.position = hit.transform.position;
                _activeAppliance.OnPlaceFood(this);

                if (_cameFromFridge)
                {
                Fridge fridge = FindAnyObjectByType<Fridge>();

                    if (fridge != null)
                    {
                        fridge.SpawnFood();
                    }

                _cameFromFridge = false;
            }


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
        IngredientObject info = GetComponent<IngredientObject>();

        bool stillNeedsCooking =
            info != null &&
            info.IngredientInstance != null &&
            info.IngredientInstance.Data.NeedsCooking &&
            info.IngredientInstance.CurrentCookState == CookState.Raw;

        if (_returnAppliance != null && stillNeedsCooking)
        {
            transform.position = _returnPosition;
            _activeAppliance = _returnAppliance;
            _activeAppliance.OnPlaceFood(this);

            Debug.Log("Food still needs cooking, snapping back to stove.");
            return;
        }

        if (_hasHomePosition)
        {
            transform.position = _homePosition;
            Debug.Log("Missed drop, returning food to fridge.");
        }

        _activeAppliance = null;
        _returnAppliance = null;
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
