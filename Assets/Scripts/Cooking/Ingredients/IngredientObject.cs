using UnityEngine;
using UnityEngine.UI;

public class IngredientObject : MonoBehaviour
{
    [SerializeField] private IngredientData _data;

    public Ingredient IngredientInstance { get; private set; }

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        
        if (_data.Name == "Dough") {
        IngredientInstance = new Dough(_data);
        } 
        else if (_data.Name == "Cheese") {
            IngredientInstance = new Cheese(_data); 
        }
        else {
            IngredientInstance = new GenericIngredient(_data);
        }
        UpdateSprite();
    }

    private void Update()
    {
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (_image == null) return;

        if (IngredientInstance.IsSpoiled)
        {
            _image.sprite = _data.SpoiledSprite;
            return;
        }
        switch (IngredientInstance.CurrentCookState)
        {
            case CookState.Cooked:
            case CookState.Boiled:
            case CookState.Grilled:
            case CookState.Toasted:
                _image.sprite = _data.CookedSprite;
                break;

            case CookState.Burnt:
                _image.sprite = _data.BurntSprite;
                break;

            default: // Raw
                _image.sprite = _data.RawSprite;
                break;
        }
    }
}
