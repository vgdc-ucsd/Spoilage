using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
public class FoodGrab : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static bool IsDeleteModeActive = false;
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
    private Dictionary<string, float> _stationTimers = new();
    private string _lastStationID;
    public bool IsLocked = false;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _foodImage = GetComponent<Image>();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (IsDeleteModeActive)
        {
            if (_spawner != null) return;

            CookingStation station = GetComponentInParent<CookingStation>();
            
            if (station != null)
            {
                // Station deletes all children
                station.OnPointerClick(eventData); 
            }
            else
            {
                Destroy(gameObject);
            }
            return;
        }

        if (IsLocked) return;

        TryGrab();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (IsLocked) return;
        if (!CanMoveFood || IsDeleteModeActive || _isPlaced) return;

        CookingStation station = GetComponentInParent<CookingStation>();

        if (station is AutomaticStation autoStation)
        {
            if (autoStation._isCooking && !autoStation._isOverCooking)
            {
                Debug.Log($"[FoodGrab] Cannot move {gameObject.name} while it is actively cooking!");
                return;
            }
        }

        if (_cameFromFridge)
        {
            if (_spawner != null)
            {
                _spawner.SpawnFood();
                _cameFromFridge = false;
            }
        }

        if (station != null)
        {
            _returnStation = station;
            _returnPosition = station.transform.position;

            station.OnRemoveFood();

            _activeStation = null;
        }

        _originalParent = _rectTransform.parent;
        _originalPosition = _rectTransform.anchoredPosition;

        // Bring object to top while dragging
        _rectTransform.SetParent(_canvas.transform);
        _rectTransform.SetAsLastSibling();

        Canvas foodCanvas = GetComponent<Canvas>();
        if (foodCanvas == null)
            foodCanvas = gameObject.AddComponent<Canvas>();
        foodCanvas.overrideSorting = true;
        foodCanvas.sortingOrder = 100;
        
        if (GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        if (_foodImage != null) _foodImage.raycastTarget = false;
        
        TryGrab();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (IsLocked) return;
        if (!CanMoveFood || _isPlaced) return;

        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (IsLocked) return;
        if (!CanMoveFood || _isPlaced) return;

        if (_foodImage != null) _foodImage.raycastTarget = true;

        Canvas foodCanvas = GetComponent<Canvas>();
        if (foodCanvas != null)
            foodCanvas.overrideSorting = false;

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
        if (IsLocked) return false;
        if (!CanMoveFood || _isPlaced) return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = RectTransformUtility.WorldToScreenPoint(null, _rectTransform.position)
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        KitchenTile tile = GetTileFromRaycast(results);

        if (tile != null)
        {            
            if (tile.GetTopObject() != gameObject) return false;

            // Unplate ingredients upon pickup
            GameObject curr = tile.GetTopObject();
            if (curr.TryGetComponent<IngredientObject>(out IngredientObject component))
            {
                IngredientBehaviour currBehaviour = curr.GetComponent<IngredientBehaviour>();
                currBehaviour.UnplateIngredient();
            }

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
        if (_foodImage != null) _foodImage.raycastTarget = true;

        bool foundValidDrop = false;
        bool blockedByFullStation = false;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        Debug.Log($"UI Raycast hits: {results.Count}");
        foreach (var r in results)
            Debug.Log($"  hit: {r.gameObject.name}");

        foreach (var result in results)
        {
            GameObject hitObj = result.gameObject;

            TrashCan trash = hitObj.GetComponentInParent<TrashCan>();
            if (trash != null)
            {
                trash.Trash(this);
                foundValidDrop = true;
                break;
            }

            Plate plate = hitObj.GetComponentInParent<Plate>();
            if (plate != null)
            {
                Debug.Log("Plate found!");
                IngredientObject info = GetComponent<IngredientObject>();

                if (info != null && plate.AddIngredient(info))
                {
                    plate.PrintIngredients();
                    _isPlaced = true;
                    _rectTransform.SetParent(plate.transform);
                    _rectTransform.anchoredPosition = Vector2.zero;
                    foundValidDrop = true;
                    break;
                }
            }

            CookingStation app = hitObj.GetComponentInParent<CookingStation>();
            if (app != null)
            {
                if (!app.HasSpace)
                {
                    Debug.Log($"{app.gameObject.name} is full. Cannot drop food here.");
                    blockedByFullStation = true;
                    break;
                }

                _rectTransform.SetParent(app.transform, false);
                _rectTransform.anchoredPosition = Vector2.zero;

                bool placed = app.OnPlaceFood(this);

                if (placed)
                {
                    _activeStation = app;
                    foundValidDrop = true;
                }
                else
                {
                    // remove this rejected food from any tile tracking
                    KitchenTile originalTile = _originalParent.GetComponent<KitchenTile>();
                    if (originalTile != null)
                    {
                        originalTile.RemoveObject(gameObject);
                    }
                    // snap back without triggering tile placement logic
                    _rectTransform.SetParent(_originalParent, false);
                    _rectTransform.anchoredPosition = _originalPosition;

                    _activeStation = null;
                    _returnStation = null;
                }

                break;
            }

            UtilityStation util = hitObj.GetComponentInParent<UtilityStation>();
            if (util != null)
            {
                _rectTransform.SetParent(util.transform, false);
                _rectTransform.anchoredPosition = Vector2.zero;
                util.OnPlaceFood(this);
                foundValidDrop = true;
                break;
            }
        }

        if (blockedByFullStation)
        {
            ReturnToHome();
            return;
        }

        if (foundValidDrop)
        {
            if (_cameFromFridge)
            {
                FoodSpawner foodSpawner = FindAnyObjectByType<FoodSpawner>();
                if (foodSpawner != null) foodSpawner.SpawnFood();
                _cameFromFridge = false;
            }

            return;
        }

        KitchenTile targetTile = GetTileFromRaycast(results);
        if (targetTile != null && targetTile.CanPlaceObject("Food", gameObject))
        {
            targetTile.PlaceObject(gameObject);
            _rectTransform.SetParent(targetTile.GetComponent<RectTransform>() != null 
                ? targetTile.transform : _originalParent, false);
            _rectTransform.anchoredPosition = Vector2.zero;
            _returnStation = null;
            return;
        }

        ReturnToHome();
    }

    private void ReturnToHome()
    {
        IngredientObject info = GetComponent<IngredientObject>();
        
        if (_cameFromFridge)
        {
            _rectTransform.SetParent(_originalParent);
            _rectTransform.anchoredPosition = _originalPosition;

            // Add back to tile
            KitchenTile originalTile = _originalParent.GetComponent<KitchenTile>();
            originalTile.PlaceObject(info.gameObject);

            // // Plates object when snapping back on
            // if(_originalParent.name.Contains("KitchenTile"))
            // {
            //     IngredientBehaviour behaviour = info.GetComponent<IngredientBehaviour>();
            //     behaviour.PlateIngredient();
            // }


            Debug.Log("Invalid drop, returning fridge food home.");
            return;
        }

        if (_returnStation != null)
        {
            _rectTransform.SetParent(_returnStation.transform, false);
            _rectTransform.anchoredPosition = Vector2.zero;
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

        KitchenTile tile = _originalParent.GetComponent<KitchenTile>();
        if (tile != null)
        {
            tile.PlaceObject(gameObject); // handles PutOnSpoilSurface
            Debug.Log("Invalid drop, snapping back to kitchen tile.");
            return;
        }

        _rectTransform.SetParent(_originalParent);
        _rectTransform.anchoredPosition = _originalPosition;

        _activeStation = null;
        _returnStation = null;
    }

    private KitchenTile GetTileFromRaycast(List<RaycastResult> results)
    {
        foreach (var result in results)
        {
            KitchenTile tile = result.gameObject.GetComponentInParent<KitchenTile>();
            if (tile != null) return tile;
        }
        return null;
    }

    public void LockToPlate()
    {
        _isPlaced = true;
    }

    public void SaveCookTimer(string stationID, float timer)
    {
        _stationTimers[stationID] = timer;
    }

    public float GetCookTimer(string stationID)
    {
        return _stationTimers.TryGetValue(stationID, out float t) ? t : 0f;
    }

    public void ClearCookTimers()
    {
        _stationTimers.Clear();
    }

    public void SetLastStation(string stationID)
    {
        _lastStationID = stationID;
    }

}