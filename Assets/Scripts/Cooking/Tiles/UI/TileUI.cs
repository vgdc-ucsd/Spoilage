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
    
    public virtual void OnPointerEnter(PointerEventData _)
    {
        DragAndDropManager.Instance.Hover(this);    
    }
    
    public virtual void OnPointerExit(PointerEventData _)
    {
        DragAndDropManager.Instance.Unhover();
    }

    public virtual void OnBeginDrag(PointerEventData _)
    {
        DragAndDropManager.Instance.BeginDrag(this);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        DragAndDropManager.Instance.Drag(eventData);    
    }

    public virtual void OnEndDrag(PointerEventData _)
    {
        DragAndDropManager.Instance.EndDrag();
    }

    public virtual void OnPointerDown(PointerEventData _)
    {
        DragAndDropManager.Instance.Click();
    }
}
