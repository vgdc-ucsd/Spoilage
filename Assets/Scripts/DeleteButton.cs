using UnityEngine;
using UnityEngine.UI;

public class DeleteButton : MonoBehaviour
{
    [SerializeField] private Color _activeColor = Color.red;
    [SerializeField] private Color _inactiveColor = Color.white;
    
    private Image _buttonImage;
    private bool _isActive = false;

    void Awake()
    {
        _buttonImage = GetComponent<Image>();
        FoodGrab.IsDeleteModeActive = false;
    }

    public void ToggleDeleteMode()
    {
        _isActive = !_isActive;
        FoodGrab.IsDeleteModeActive = _isActive;
        
        // Disables normal food movement while deleting
        FoodGrab.CanMoveFood = !_isActive;

        // Visual feedback
        if (_buttonImage != null)
        {
            _buttonImage.color = _isActive ? _activeColor : _inactiveColor;
        }

        Debug.Log($"[Delete Mode] Status: {(_isActive ? "ENABLED" : "DISABLED")}");
    }
}