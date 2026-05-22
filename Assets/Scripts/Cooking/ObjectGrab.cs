using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectGrab : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static bool CanMoveAppliances = true;

    public KitchenTile currentTile;

    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Image _applianceImage;
    private Vector2 _originalPosition;
    private Transform _originalParent;
    private KitchenTile _sourceTileBeforeDrag;

    private void Awake()
    {
        KitchenTile parentTile = GetComponentInParent<KitchenTile>();
        if (parentTile != null)
        {
            currentTile = parentTile;
            if (!parentTile.objectsOnTile.Contains(gameObject))
                parentTile.objectsOnTile.Add(gameObject);
        }

        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _applianceImage = GetComponent<Image>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (!CanMoveAppliances || LockLayout.IsLocked) return;
        _sourceTileBeforeDrag = currentTile;
        if (!TryGrab()) return;

        _originalParent = _rectTransform.parent;
        _originalPosition = _rectTransform.anchoredPosition;

        // Bring to top of canvas while dragging
        _rectTransform.SetParent(_canvas.transform);
        _rectTransform.SetAsLastSibling();

        Canvas applianceCanvas = GetComponent<Canvas>();
        if (applianceCanvas == null)
            applianceCanvas = gameObject.AddComponent<Canvas>();
        applianceCanvas.overrideSorting = true;
        applianceCanvas.sortingOrder = 100;

        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();

        if (_applianceImage != null) _applianceImage.raycastTarget = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!CanMoveAppliances || LockLayout.IsLocked) return;

        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (!CanMoveAppliances || LockLayout.IsLocked) return;

        if (_applianceImage != null) _applianceImage.raycastTarget = true;

        Drop(eventData);
    }

    public bool TryGrab()
    {
        if (!CanMoveAppliances || LockLayout.IsLocked) return false;

        // Block if food is currently on this appliance
        if (GetComponentInChildren<FoodGrab>() != null)
        {
            Debug.Log("Cannot move appliance: food is on it.");
            return false;
        }

        // Remove from current tile
        if (currentTile != null)
        {
            if (currentTile.GetTopObject() != gameObject) return false;
            currentTile.RemoveObject(gameObject);
        }

        return true;
    }

    public void Drop(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        KitchenTile targetTile = GetTileFromRaycast(results);

        // This lets you swap the appliances
        if (targetTile != null && targetTile.HasAppliance() && _sourceTileBeforeDrag != null)
        {
            GameObject otherAppliance = targetTile.GetTopObject();
            ObjectGrab otherGrab = otherAppliance != null ? otherAppliance.GetComponent<ObjectGrab>() : null;

            if (otherGrab != null)
            {
                targetTile.RemoveObject(otherAppliance);

                _sourceTileBeforeDrag.PlaceObject(otherAppliance);
                otherGrab.currentTile = _sourceTileBeforeDrag;

                targetTile.PlaceObject(gameObject);
                currentTile = targetTile;
                return;
            }
        }

        // This is basically the standard way to place things
        if (targetTile != null && targetTile.CanPlaceObject("Appliance", gameObject))
        {
            targetTile.PlaceObject(gameObject);
            currentTile = targetTile;
            return;
        }

        // Snaps back if placement fails
        if (_sourceTileBeforeDrag != null)
        {
            _sourceTileBeforeDrag.PlaceObject(gameObject);
            currentTile = _sourceTileBeforeDrag;
            return;
        }

        _rectTransform.SetParent(_originalParent, false);
        _rectTransform.anchoredPosition = _originalPosition;
        Debug.Log("Invalid placement: returning to original position.");
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
}