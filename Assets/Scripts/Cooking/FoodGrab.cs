using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
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
    private CanvasGroup _canvasGroup;
    private Image _foodImage;
    private Vector2 _originalPosition;
    private Transform _originalParent;

    private float _savedCookTimer;

    public static bool CanMoveFood = true;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _foodImage = GetComponent<Image>();

        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        //TryGrab();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (!CanMoveFood || _isPlaced) return;

        _originalParent = _rectTransform.parent;
        _originalPosition = _rectTransform.anchoredPosition;

        if (_activeStation != null)
        {
            _returnStation = _activeStation;
            _returnPosition = _activeStation.transform.position;
            _activeStation.RemoveFood(this);
            _activeStation = null;
        }

        if (!TryGrab())
        {
            return;
        }

        //  bring object to top while dragging
        _rectTransform.SetParent(_canvas.transform);
        _rectTransform.SetAsLastSibling();

        _canvasGroup.blocksRaycasts = true;

        if (_foodImage != null) _foodImage.raycastTarget = false;


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
            tile.RemoveObject(gameObject);
        }


        // Clean up appliance reference if we pick it back up
        if (_activeStation != null)
        {
            _returnStation = _activeStation;
            _returnPosition = _activeStation.transform.position;

            _activeStation.RemoveFood(this);
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
        if (_foodImage != null) _foodImage.raycastTarget = true;

        bool foundValidDrop = false;

        // Use UI raycast results — all objects the pointer passed over
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
                    plate.PrintIngredient();
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
                _activeStation = app;
                _activeStation.PlaceFood(this);
                foundValidDrop = true;
                break;
            }
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
            _activeStation = null;
            _returnStation = null;
            return;
        }

        ReturnToHome();
    }

    private void ReturnToHome()
    {
        IngredientObject info = GetComponent<IngredientObject>();

        bool stillNeedsCooking =
            info != null &&
            info.IngredientInstance != null;
        
        if (_cameFromFridge)
        {
            _rectTransform.SetParent(_originalParent);
            _rectTransform.anchoredPosition = _originalPosition;
            Debug.Log("Invalid drop, returning fridge food home.");
            return;
        }

        if (_spawner == null) 
        {
            Debug.Log("Duplicate or invalid fridge item, destroying.");
            Destroy(gameObject); 
            return;
        }

        // if (_returnStation != null && stillNeedsCooking)
        // {
        //     _rectTransform.position = _returnPosition;
        //     _activeStation = _returnStation;
        //     _activeStation.OnPlaceFood(this);

        //     Debug.Log("Food still needs cooking, snapping back to stove.");
        //     return;
        // }

        if (_hasHomePosition)
        {
            _rectTransform.SetParent(_originalParent);
            _rectTransform.anchoredPosition = _originalPosition;
            Debug.Log("Missed drop, returning food to fridge.");
        }

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

    public void SaveCookTimer(float timer)
    {
        _savedCookTimer = timer;
    }

    public float GetSavedCookTimer()
    {
        return _savedCookTimer;
    }

    public void ClearSavedCookTimer()
    {
        _savedCookTimer = 0f;
    }
}
