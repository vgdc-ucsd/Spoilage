using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private ObjectGrab _currentDragging;

    void Update()
    {
        // 1. Detect Mouse Press
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }

        // 2. Handle Dragging
        if (_currentDragging != null)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                _currentDragging.UpdateDragPosition();
            }
            else
            {
                _currentDragging.Drop();
                _currentDragging = null;
            }
        }
    }

    void HandleClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);

        ObjectGrab target = null;

        // PRIORITY: Check for Appliance first, then Counter
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out ObjectGrab obj) && obj.type == ObjectGrab.StationType.Appliance)
            {
                target = obj;
                break;
            }
        }

        if (target == null)
        {
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out ObjectGrab obj) && obj.type == ObjectGrab.StationType.Counter)
                {
                    target = obj;
                    break;
                }
            }
        }

        if (target != null && target.TryGrab())
        {
            _currentDragging = target;
        }
    }
}
