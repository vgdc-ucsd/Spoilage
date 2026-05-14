using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectGrab : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static bool CanMoveAppliances = true;

    public KitchenTile currentTile;

    protected RectTransform _rectTransform;
    protected Canvas _canvas;
    protected Image _applianceImage;
    protected Vector2 _originalPosition;
    protected Transform _originalParent;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _applianceImage = GetComponent<Image>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (!CanMoveAppliances || LockLayout.IsLocked) return;
        if (!TryGrab()) return;

        _originalParent = _rectTransform.parent;
        _originalPosition = _rectTransform.anchoredPosition;

        // Bring to top of canvas while dragging
        _rectTransform.SetParent(_canvas.transform);
        _rectTransform.SetAsLastSibling();

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

        if (targetTile != null && targetTile.CanPlaceObject("Appliance", gameObject))
        {
            targetTile.PlaceObject(gameObject);
            currentTile = targetTile;
            return;
        }

        // Snap back to last valid tile
        if (currentTile != null)
        {
            currentTile.PlaceObject(gameObject);
            return;
        }

        // No valid tile at all, return to original position
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