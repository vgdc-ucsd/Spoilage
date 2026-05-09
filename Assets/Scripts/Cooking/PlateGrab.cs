using UnityEngine.EventSystems;

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
}
