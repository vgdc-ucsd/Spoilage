using UnityEngine;
using UnityEngine.InputSystem;

public class InteractManager : MonoBehaviour
{
    private MonoBehaviour _currentDragging;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) HandleClick();

        if (_currentDragging != null)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                // Dragging Logic: The 'is' keyword casts the object safely
                if (_currentDragging is PlateGrab plate) plate.UpdateDragPosition();
                else if (_currentDragging is FoodGrab food) food.UpdateDragPosition();
                else if (_currentDragging is ObjectGrab station) station.UpdateDragPosition();
            }
            else
            {
                // Drop Logic
                if (_currentDragging is PlateGrab plate) plate.Drop();
                else if (_currentDragging is FoodGrab food) food.Drop();
                else if (_currentDragging is ObjectGrab station) station.Drop();

                _currentDragging = null;
            }
        }
    }

    void HandleClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);

        // 1. Check for the "Start Day" Button
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out WorldButton button))
            {
                button.Trigger();
                return; // Stop here if we hit the button
            }
        }

        // 2. Check for Food (Always moveable)
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out FoodGrab food) && food.TryGrab())
            {
                _currentDragging = food;
                return;
            }
        }

        // 3. THE LOCK GATE
        if (LockLayout.IsLocked) return;

        // 4. Check for Stations/Counters (Only reached if IsLocked is false)
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out ObjectGrab obj) && obj.TryGrab())
            {
                _currentDragging = obj;
                return;
            }
        }
    }
}
