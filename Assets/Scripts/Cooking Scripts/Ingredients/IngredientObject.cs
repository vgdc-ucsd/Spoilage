using UnityEngine;

public class IngredientObject : MonoBehaviour
{
    [SerializeField] private IngredientData _data;

    public Ingredient IngredientInstance { get; private set; }
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        IngredientInstance = new Dough(_data); 
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    private void Update()
    {
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        switch (IngredientInstance.CurrentState)
        {
            case IngredientState.Raw:
                _spriteRenderer.sprite = _data.RawSprite;
                break;

            case IngredientState.Cooked:
                _spriteRenderer.sprite = _data.CookedSprite;
                break;

            case IngredientState.Burnt:
                _spriteRenderer.sprite = _data.BurntSprite;
                break;

        }
    }
}
