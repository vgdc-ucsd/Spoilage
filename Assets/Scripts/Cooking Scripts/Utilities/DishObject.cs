using UnityEngine;

public class DishObject : MonoBehaviour
{
    [SerializeField] private DishData _data;

    public Dish DishInstance { get; private set; }
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        DishInstance = new Flatbread(_data); 
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    private void Update()
    {
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        // Where sprites??
        switch (DishInstance.CurrentState)
        {
            case DishState.Normal:
                _spriteRenderer.sprite = _data.NormalSprite;
                break;

            case DishState.Spoiled:
                _spriteRenderer.sprite = _data.SpoiledSprite;
                break;
            case DishState.Burnt:
                _spriteRenderer.sprite = _data.BurntSprite;
                break;

            case DishState.Seasoned:
                _spriteRenderer.sprite = _data.SeasonedSprite;
                break;

        }
    }
}
