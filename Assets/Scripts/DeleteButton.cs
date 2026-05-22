using UnityEngine;
using UnityEngine.UI;

public class DeleteButton : MonoBehaviour
{
    private bool _isActive = false;
    private Image _img;

    void Awake() => _img = GetComponent<Image>();

    public void ToggleDeleteMode()
    {
        _isActive = !_isActive;
        FoodGrab.IsDeleteModeActive = _isActive;
        FoodGrab.CanMoveFood = !_isActive;
        
        if (_img != null) _img.color = _isActive ? Color.red : Color.white;
        Debug.Log($"[Delete Mode] Active: {_isActive}");
    }
}