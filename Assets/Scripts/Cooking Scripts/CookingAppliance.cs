using UnityEngine;

public class CookingAppliance : MonoBehaviour
{
    [Header("Base Appliance Settings")]
    // This basically tells us the appliance if it's a toaster (Toasted), grill (Grilled), etc.
    [SerializeField] protected CookState targetState;


    // These virtual methods allow child scripts to add their own unique logic
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
        Debug.Log($"{gameObject.name}: Food placed.");
        
    }

    public virtual void OnRemoveFood()
    {
        Debug.Log($"{gameObject.name}: Food removed.");
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
