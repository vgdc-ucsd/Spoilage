//using UnityEngine.InputSystem;
//using UnityEngine;

//public class FoodGrab : MonoBehaviour
//{
//    [SerializeField] private Transform _homeSpot;
//    [SerializeField] private Transform _plateSpot;
//    private StoveTops _activeStove;
//    private Countertops _activeCountertop;
//    private CookingAppliance _activeAppliance;
//    private bool _isPlaced = false;
//    private Collider2D _col;

//    public bool TryGrab()
//    {
//        // Check if we are on a tile and remove from list if so
//        KitchenTile tile = GetTileAtPosition(transform.position);
//        if (tile != null)
//        {
//            if (tile.GetTopObject() != gameObject) return false;
//            tile.RemoveObject(gameObject);
//        }

//        // Clean up stove reference
//        if (_activeAppliance != null)
//        {
//            _activeAppliance.OnRemoveFood();
//            _activeAppliance = null;
//        }
//        return true;
//    }

//    //On Click: Get mouse position to set up for dragging
//    //private void OnMouseDown()
//    //{
//    //    if (_isPlaced) return;
//    //    if (_activeAppliance != null)
//    //    {
//    //        _activeStove.OnRemoveFood();
//    //        _activeStove = null;
//    //    } else if(_activeCountertop != null)
//    //    {
//    //        _activeCountertop.OnRemoveFood();
//    //        _activeCountertop = null;
//    //    }
//    //    return;
//    //}

//    public void UpdateDragPosition()
//    {
//        if (_isPlaced) return;

//        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
//        // Z -3 keeps food visually above appliances
//        transform.position = new Vector3(mousePos.x, mousePos.y, -3f);
//    }

//    public void Drop()
//    {
//        if (_isPlaced) return;
//        //If not at stove top, revert back to original position
//        //(Keep track of original position)
//        // _col.enabled = false;
//        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
//        // _col.enabled = true;
//        foreach (Collider2D hit in hits)
//        {
//            CookingAppliance _app = hit.GetComponentInParent<CookingAppliance>();

//            if (_app != null)
//            {
//                _activeAppliance = _app;
//                transform.position = hit.transform.position;
//                Debug.Log("Snapped to: " + hit.name);
//                _activeAppliance.OnPlaceFood(this);
//                return;
//            }

//            /*else if (hit.gameObject.name.Contains("Plate"))
//            {
//                IngredientObject _currentFood = GetComponent<IngredientObject>();
//                if (_currentFood.IngredientInstance.CurrentState == IngredientState.Cooked)
//                {
//                    transform.position = hit.transform.position;
//                    Debug.Log("Snapped to: " + hit.name);
//                    return;
//                }
//            } else if (hit.gameObject.name.Contains("Countertop"))
//            {
//                Countertops _countertop = hit.GetComponent<Countertops>();
//                if (_countertop != null)
//                {
//                    _activeCountertop = _countertop;
//                    transform.position = hit.transform.position;
//                    Debug.Log("Snapped to: " + hit.name);
//                    _activeCountertop.OnPlaceFood(this);
//                    return;
//                }
//            }*/

//            else if (hit.GetComponentInParent<Plate>() != null)
//            {
//                Plate plate = hit.GetComponentInParent<Plate>();
//                IngredientObject food = GetComponent<IngredientObject>();

//                plate.AddIngredient(food);

//                _activeAppliance = null;

//                plate.PrintIngredients();
//                return;
//            }
//        }

//        // --- 2. TILE-BASED LOGIC (For Stoves and Grid Counters) ---
//        KitchenTile targetTile = null;
//        foreach (var hit in hits)
//        {
//            transform.position = _homeSpot.position;
//            _activeStove = null;
//            _activeCountertop = null;
//            _activeAppliance = null;
//            if (hit.TryGetComponent(out targetTile)) break;
//        }

//        if (targetTile != null && targetTile.CanPlaceObject("Food", gameObject))
//        {
//            foreach (var hit in hits)
//            {
//                // STOVE/POT CHECK
//                if (hit.gameObject.name.Contains("StoveTop") || hit.gameObject.name.Contains("Pot"))
//                {
//                    CookingAppliance app = hit.GetComponentInParent<CookingAppliance>();
//                    if (app != null)
//                    {
//                        targetTile.PlaceObject(gameObject, "Food");
//                        _activeAppliance = app;
//                        transform.position = hit.transform.position;
//                        _activeAppliance.OnPlaceFood(this);
//                        return;
//                    }
//                }
//            }

//            // Fallback: Drop on the counter tile itself
//            targetTile.PlaceObject(gameObject, "Food");
//            transform.position = targetTile.transform.position;
//            return;
//        }

//        // --- 3. RETURN TO HOME (If no valid landing spot found) ---
//        ReturnToHome();
//    }

//    private void ReturnToHome()
//    {
//        if (_homeSpot != null) transform.position = _homeSpot.position;
//        _activeAppliance = null;
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

//    public void LockToPlate()
//    {
//        _isPlaced = true;

//        if (_col != null)
//            _col.enabled = false;
//    }


//}
using UnityEngine.InputSystem;
using UnityEngine;

public class FoodGrab : MonoBehaviour
{
    [SerializeField] private Transform _homeSpot;
    private StoveTops _activeStove;
    private Countertops _activeCountertop;
    private CookingAppliance _activeAppliance;
    private bool _isPlaced = false;
    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
    }

    public bool TryGrab()
    {
        // 1. Tile Cleanup
        KitchenTile tile = GetTileAtPosition(transform.position);
        if (tile != null)
        {
            // Only grab if this is the top object on the tile
            if (tile.GetTopObject() != gameObject) return false;
            tile.RemoveObject(gameObject);
        }

        // 2. Appliance Cleanup
        if (_activeAppliance != null)
        {
            _activeAppliance.OnRemoveFood();
            _activeAppliance = null;
        }

        return true;
    }

    public void UpdateDragPosition()
    {
        if (_isPlaced) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        // Z -3 keeps food visually above appliances (Z -1) and tiles (Z 0)
        transform.position = new Vector3(mousePos.x, mousePos.y, -3f);
    }

    public void Drop()
    {
        if (_isPlaced) return;

        // Use a small radius to detect what's under the food
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        // --- 1. PLATE LOGIC ---
        foreach (Collider2D hit in hits)
        {
            Plate plate = hit.GetComponentInParent<Plate>();
            if (plate != null)
            {
                IngredientObject foodItem = GetComponent<IngredientObject>();
                plate.AddIngredient(foodItem);
                _activeAppliance = null;
                return; // Exit Drop early if placed on plate
            }
        }

        // --- 2. TILE & APPLIANCE LOGIC ---
        KitchenTile targetTile = null;
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out targetTile)) break;
        }

        // If we found a tile, check if we can place "Food" there
        if (targetTile != null && targetTile.CanPlaceObject("Food", gameObject))
        {
            targetTile.PlaceObject(gameObject); // Fixed: Only 1 argument now

            // Check if there is an appliance on this tile to snap to
            foreach (var hit in hits)
            {
                CookingAppliance app = hit.GetComponentInParent<CookingAppliance>();
                if (app != null)
                {
                    _activeAppliance = app;
                    transform.position = hit.transform.position;
                    _activeAppliance.OnPlaceFood(this);
                    return;
                }
            }

            // Fallback: Just sit in the center of the tile
            transform.position = targetTile.transform.position;
        }
        else
        {
            // --- 3. RETURN TO HOME ---
            ReturnToHome();
        }
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
        if (_col != null) _col.enabled = false;
    }
}
