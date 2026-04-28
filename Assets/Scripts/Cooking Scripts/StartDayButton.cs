using UnityEngine;
using UnityEngine.Events;

public class WorldButton : MonoBehaviour
{
    public UnityEvent OnClick;
    private SpriteRenderer _renderer;
    private Color _originalColor;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer != null) _originalColor = _renderer.color;
    }

    public void Trigger()
    {
        OnClick.Invoke();
    }

    // Visual feedback when hovering
    void OnMouseEnter() { if (_renderer) _renderer.color = Color.gray; }
    void OnMouseExit() { if (_renderer) _renderer.color = _originalColor; }

    public void OnStartDayButtonPressed()
    {
        // This unlocks all FoodGrab scripts at once
        FoodGrab.CanMoveFood = true;

        Debug.Log("You can cook now!");
    }
}
