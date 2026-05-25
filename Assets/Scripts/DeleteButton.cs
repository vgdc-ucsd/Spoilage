using UnityEngine;
using UnityEngine.UI;

public class DeleteButton : MonoBehaviour
{
    [SerializeField] private GameObject _selectedImage;
    private bool _isActive = false;

    void Awake()
    {
        if (_selectedImage != null) _selectedImage.SetActive(false);
    }

    public void ToggleDeleteMode()
    {
        _isActive = !_isActive;
        FoodGrab.IsDeleteModeActive = _isActive;
        FoodGrab.CanMoveFood = !_isActive;
        
        if (_selectedImage != null) _selectedImage.SetActive(_isActive);
        Debug.Log($"[Delete Mode] Active: {_isActive}");
    }
}
