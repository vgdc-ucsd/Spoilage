using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class FoodGrab : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private bool _hasHomePosition;
    private bool _isPlaced = false;
    private bool _cameFromFridge = true;
    private Vector3 _homePosition;
    private Vector3 _returnPosition;
    [SerializeField] private Transform _plateSpot;
    private FoodSpawner _spawner;
    private CookingStation _activeStation;
    private CookingStation _returnStation;

    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Image _foodImage;
    private Vector2 _originalPosition;
    private Transform _originalParent;

    public static bool CanMoveFood = true;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _foodImage = GetComponent<Image>();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        TryGrab();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (!CanMoveFood || _isPlaced) return;

        _originalParent = _rectTransform.parent;
        _originalPosition = _rectTransform.anchoredPosition;

        // bring object to top while dragging
        _rectTransform.SetParent(_canvas.transform);
        _rectTransform.SetAsLastSibling();

        if (_foodImage != null) _foodImage.raycastTarget = false;

        TryGrab();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!CanMoveFood || _isPlaced) return;

        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (!CanMoveFood || _isPlaced) return;

        if (_foodImage != null) _foodImage.raycastTarget = true;

        Drop(eventData);
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
        Debug.Log($"TryGrab called | CanMove: {CanMoveFood} | isPlaced: {_isPlaced}");
        if (!CanMoveFood || _isPlaced) return false;

        // Check if we are on a tile and remove from list if so
        //KitchenTile tile = GetTileAtPosition(transform.position);
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(_rectTransform.position);
        KitchenTile tile = GetTileAtPosition(worldPos);

        if (tile != null)
        {
            if (tile.GetTopObject() != gameObject) return false;
            tile.RemoveObject(gameObject);
        }

        // Clean up appliance reference if we pick it back up
        if (_activeStation != null)
        {
            _returnStation = _activeStation;
            _returnPosition = _activeStation.transform.position;

            _activeStation.OnRemoveFood();
            _activeStation = null;
        }
        else
        {
            _returnStation = null;
        }


        if (_spawner != null)
        {
            _spawner.ClearCurrentFood(this);
            _spawner.SpawnFood();
            _spawner = null;
        }

        return true;
    }

    public void SetSpawner(FoodSpawner spawner)
    {
        _spawner = spawner;
    }
    public void Drop(PointerEventData eventData)
    {
        if (_isPlaced) return;

        // INCREASE the radius to 1.0f to make it easier to hit the plate
        //Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.0f);
        GameObject hit = eventData.pointerCurrentRaycast.gameObject;
        bool foundValidDrop = false;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);

        //foreach (Collider2D hit in hits)
        if (hit != null)
        {
            Debug.Log("Food dropped over: " + hit.gameObject.name);

            /*FoodGrab otherFood = hit.GetComponentInParent<FoodGrab>();
            if (otherFood != null && otherFood != this && otherFood._isPlaced)
                continue;*/

            // --- 1. SCAN FOR PLATE OR TRASH OR APPLIANCE ---
            TrashCan trash = hit.GetComponentInParent<TrashCan>();
            if (trash != null)
            {
                trash.Trash(this);
                foundValidDrop = true;
            }

            // This looks for the Plate script anywhere on the object we hit or its parents
            Plate plate = hit.GetComponentInParent<Plate>();

            if (plate != null)
            {
                Debug.Log("!!! PLATE DETECTED !!!"); // Look for this in the console!
                IngredientObject info = GetComponent<IngredientObject>();
                if (info != null)
                {
                    if (plate.AddIngredient(info)) 
                    {
                        plate.PrintIngredients();
                        _isPlaced = true;

                        _rectTransform.SetParent(plate.transform);
                        _rectTransform.anchoredPosition = Vector2.zero;

                        foundValidDrop = true;
                        //break;
                    }
                    else
                    {
                        foundValidDrop = false;
                        //break;
                    }
                }
            }

            CookingStation app = hit.GetComponentInParent<CookingStation>();
            if (app != null)
            {
                _activeStation = app;
                //transform.position = hit.transform.position;
                _rectTransform.SetParent(app.transform, false);

                _rectTransform.anchoredPosition =Vector2.zero;

                _activeStation.OnPlaceFood(this);

                KitchenTile tile = GetTileAtPosition(worldPos);
                if (tile != null) tile.PlaceObject(gameObject);
                foundValidDrop = true;
            }
        }

        if (foundValidDrop)
        {
            // always spawn a new food item if this came from the fridge, regardless of where it was dropped
            if (_cameFromFridge)
            {
                FoodSpawner foodSpawner = FindAnyObjectByType<FoodSpawner>();

                if (foodSpawner != null)
                {
                    foodSpawner.SpawnFood();
                }

                _cameFromFridge = false;
            }
            return;
        }

        // --- 2. TILE-BASED FALLBACK ---
        KitchenTile targetTile = GetTileAtPosition(worldPos);
        if (targetTile != null && targetTile.CanPlaceObject("Food", gameObject))
        {
            targetTile.PlaceObject(gameObject);
            _rectTransform.position = targetTile.transform.position;
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
        
        if (_cameFromFridge || _spawner == null) 
        {
            Debug.Log("Duplicate or invalid fridge item, destroying.");
            Destroy(gameObject); 
            return;
        }

        if (_returnStation != null && stillNeedsCooking)
        {
            _rectTransform.position = _returnPosition;
            _activeStation = _returnStation;
            _activeStation.OnPlaceFood(this);

            Debug.Log("Food still needs cooking, snapping back to stove.");
            return;
        }

        if (_hasHomePosition)
        {
            _rectTransform.SetParent(_originalParent);
            _rectTransform.anchoredPosition = _originalPosition;
            Debug.Log("Missed drop, returning food to fridge.");
        }

        _activeStation = null;
        _returnStation = null;
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
