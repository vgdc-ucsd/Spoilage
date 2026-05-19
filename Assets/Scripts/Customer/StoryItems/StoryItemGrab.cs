using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class StoryItemGrab : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public KitchenTile currentTile;

    protected RectTransform _rectTransform;
    protected Canvas _canvas;
    protected Image _image;
    protected Vector2 _originalPosition;
    protected Transform _originalParent;

    public void Initialize()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _image = GetComponentInChildren<Image>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (!TryGrab()) return;

        _originalParent = _rectTransform.parent;
        _originalPosition = _rectTransform.anchoredPosition;

        // Bring to top of canvas while dragging
        _rectTransform.SetParent(_canvas.transform);
        _rectTransform.SetAsLastSibling();

        if (_image != null) _image.raycastTarget = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Drop(eventData);

        if (_image != null) _image.raycastTarget = true;
    }

    public bool TryGrab()
    {
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

        if (targetTile != null && targetTile.CanPlaceObject("StoryItem", gameObject))
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
