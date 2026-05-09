using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlateGrab : ObjectGrab, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
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
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (_applianceImage != null) _applianceImage.raycastTarget = true;

        Drop(eventData);
    }

    public new bool TryGrab()
    {
        // Remove from current tile
        if (currentTile != null)
        {
            if (currentTile.GetTopObject() != gameObject) return false;
            currentTile.RemoveObject(gameObject);
        }

        return true;
    }

    public new void Drop(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        KitchenTile targetTile = GetTileFromRaycast(results);

        if (targetTile != null)
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
