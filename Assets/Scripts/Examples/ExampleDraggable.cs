using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ExampleDraggable : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _image;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Vector2 _originalPosition;
    private Transform _originalParent;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    // Calls when we click on the object
    // Note that this will also trigger when we release the mouse button after dragging,
    // since that's technically a click as well
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        // Random color on click to show interaction
        _image.color = new Color(Random.value, Random.value, Random.value);
    }

    // Calls when we start dragging the object
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        // Store original parent and position so we can return if needed
        _originalParent = _rectTransform.parent;
        _originalPosition = _rectTransform.anchoredPosition;

        // In the canvas, Images that are later siblings are rendered on top of earlier ones
        // Move to the root of canvas and set as the last child so it appears above other UI
        _rectTransform.SetParent(_canvas.transform);
        _rectTransform.SetAsLastSibling();

        // Disable raycast target so we can detect drop targets below us
        _image.raycastTarget = false;
    }

    // Calls every frame while we are dragging the object
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        // Move the object with the mouse
        _rectTransform.anchoredPosition += eventData.delta;
    }

    // Calls when we release the mouse button after dragging
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        // Re-enable raycast target to detect clicks again
        _image.raycastTarget = true;

        // Get the object we are currently hovering over with the mouse
        // Ensure that the Image component has raycastTarget enabled to be detected
        GameObject hit = eventData.pointerCurrentRaycast.gameObject;
        if (hit == null)
        {
            // If we didn't hit anything, return to original position
            ResetPosition();
            return;
        }

        // Check if the hit object has an ExampleDragTarget component
        if (hit.TryGetComponent(out ExampleDragTarget target))
        {
            // If we hit a valid target, snap to it and call some function
            _rectTransform.SetParent(target.transform);
            // Set anchored position to zero to center it on the target
            _rectTransform.anchoredPosition = Vector2.zero;
            target.OnDrop(this);
        }
        else
        {
            // If we didn't hit a valid target, return to original position
            ResetPosition();
        }
    }

    private void ResetPosition()
    {
        _rectTransform.SetParent(_originalParent);
        _rectTransform.anchoredPosition = _originalPosition;
    }
}
