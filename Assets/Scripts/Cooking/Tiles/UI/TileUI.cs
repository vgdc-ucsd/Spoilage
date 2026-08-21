using UnityEngine;
using UnityEngine.EventSystems;

public abstract class TileUI :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler
{
    public ITile Tile { get; protected set; }
    protected bool _locked;

    public virtual void Lock(bool locked)
    {
        _locked = locked;
    }
    
    public virtual void OnPointerEnter(PointerEventData _)
    {
        if (_locked) return;
        DragAndDropManager.Instance.Hover(this);    
    }
    
    public virtual void OnPointerExit(PointerEventData _)
    {
        if (_locked) return;
        DragAndDropManager.Instance.Unhover();
    }

    public virtual void OnBeginDrag(PointerEventData _)
    {
        if (_locked) return;
        DragAndDropManager.Instance.BeginDrag(this);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (_locked) return;
        DragAndDropManager.Instance.Drag(eventData);    
    }

    public virtual void OnEndDrag(PointerEventData _)
    {
        if (_locked) return;
        DragAndDropManager.Instance.EndDrag();
    }

    public virtual void OnPointerDown(PointerEventData _)
    {
        if (_locked) return;
        DragAndDropManager.Instance.Click();
    }
}
