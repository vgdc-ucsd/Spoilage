using UnityEngine;

public class CookingAppliance : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _activeSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer.sprite = _defaultSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnPlaceFood(FoodGrab food)
    {
        
    }

    public virtual void OnRemoveFood()
    {
        
    }

    public void SetSpriteActive(bool isActive)
    {
        if (_spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer reference is missing!");
            return;
        }

        // don't change sprites for work stations that don't have both sprites assigned
        if (_activeSprite == null || _defaultSprite == null)
        {
            return;
        }

        _spriteRenderer.sprite = isActive ? _activeSprite : _defaultSprite;
    }
}
