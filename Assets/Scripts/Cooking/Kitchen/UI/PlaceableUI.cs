using UnityEngine;
using UnityEngine.UI;

public class PlaceableUI : MonoBehaviour
{
    private const float DRAG_TRANSPARENCY = 0.75f;
    [SerializeField] private Image _image;

    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public void SetDrag(bool drag)
    {
        if (drag)
        {
            _image.color = Color.white * DRAG_TRANSPARENCY;
        }
        else
        {
            _image.color = Color.white;
        }
    }
}
