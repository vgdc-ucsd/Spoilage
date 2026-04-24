using UnityEngine.InputSystem;
using UnityEngine;

public class FoodGrab : MonoBehaviour
{
    [SerializeField] private Transform _homeSpot;
    [SerializeField] private Transform _plateSpot;
    private StoveTops _activeStove;
    private Countertops _activeCountertop;
    private CookingAppliance _activeAppliance;
    private bool _isPlaced = false;

    public bool TryGrab()
    {
        // Check if we are on a tile and remove from list if so
        KitchenTile tile = GetTileAtPosition(transform.position);
        if (tile != null)
        {
            if (tile.GetTopObject() != gameObject) return false;
            tile.RemoveObject(gameObject);
        }
        return true;
    }

    //On Click: Get mouse position to set up for dragging
    private void OnMouseDown()
    {
        if (_isPlaced) return;
        if (_activeAppliance != null)
        {
            _activeStove.OnRemoveFood();
            _activeStove = null;
        } else if(_activeCountertop != null)
        {
            _activeCountertop.OnRemoveFood();
            _activeCountertop = null;
        }
    }

    public void UpdateDragPosition()
    {
        if (_isPlaced) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        // Z -3 keeps food visually above appliances
        transform.position = new Vector3(mousePos.x, mousePos.y, -3f);
    }

    public void Drop()
    {
        if (_isPlaced) return;
        //If not at stove top, revert back to original position
        //(Keep track of original position)
        // _col.enabled = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        // _col.enabled = true;
        foreach (Collider2D hit in hits)
        {
            CookingAppliance _app = hit.GetComponentInParent<CookingAppliance>();

            if (_app != null)
            {
                _activeAppliance = _app;
                transform.position = hit.transform.position;
                Debug.Log("Snapped to: " + hit.name);
                _activeAppliance.OnPlaceFood(this);
                return;
            }

            /*else if (hit.gameObject.name.Contains("Plate"))
            {
                IngredientObject _currentFood = GetComponent<IngredientObject>();
                if (_currentFood.IngredientInstance.CurrentCookState == CookState.Cooked)
                {
                    transform.position = hit.transform.position;
                    Debug.Log("Snapped to: " + hit.name);
                    return;
                }
            } else if (hit.gameObject.name.Contains("Countertop"))
            {
                Countertops _countertop = hit.GetComponent<Countertops>();
                if (_countertop != null)
                {
                    _activeCountertop = _countertop;
                    transform.position = hit.transform.position;
                    Debug.Log("Snapped to: " + hit.name);
                    _activeCountertop.OnPlaceFood(this);
                    return;
                }
            }*/

            else if (hit.GetComponentInParent<Plate>() != null)
            {
                Plate plate = hit.GetComponentInParent<Plate>();
                IngredientObject food = GetComponent<IngredientObject>();

                plate.AddIngredient(food);

                _activeAppliance = null;

                plate.PrintIngredients();
                return;
            }
        }

        // --- 2. TILE-BASED LOGIC (For Stoves and Grid Counters) ---
        KitchenTile targetTile = null;
        foreach (var hit in hits)
        {
            transform.position = _homeSpot.position;
            _activeStove = null;
            _activeCountertop = null;
            _activeAppliance = null;
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

    public void LockToPlate()
    {
        _isPlaced = true;
    }

}
