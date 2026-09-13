using UnityEngine;
using UnityEngine.UI;

public class TrashCan : MonoBehaviour
{
    public bool Active => _active;

    [SerializeField] private Sprite _offSprite;
    [SerializeField] private Sprite _onSprite;
    [SerializeField] private Image _image;
    private bool _active;

    void Start()
    {
        _active = false;
        _image.sprite = _offSprite;
    }

    public void Toggle()
    {
        _active = !_active;
        if (_active)
        {
            _image.sprite = _onSprite;
            SpoilageTriggerManager.Trigger(SpoilageCategory.RAGE);
        }
        else
        {
            _image.sprite = _offSprite;
        }
    }

    public void Trash(ITile tile)
    {
        if (tile is not KitchenTile or PlatingTile) return;
        Placeable placeable = tile.Produces();
        if (placeable == null || placeable is Station) return;

        tile.Remove();
        placeable.Destroy();
        Toggle();
    }
}
