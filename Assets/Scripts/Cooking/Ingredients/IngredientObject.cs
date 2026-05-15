using UnityEngine;
using UnityEngine.UI;

public class IngredientObject : MonoBehaviour
{
    [SerializeField] private IngredientData _data;
    [SerializeField] private Image _image;

    public Ingredient IngredientInstance { get; private set; }

    private void Awake()
    {
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }

        if (_image == null)
        {
            _image = GetComponentInChildren<Image>();
        }

        IngredientInstance = new Ingredient(_data);
        UpdateSprite();
    }

    private void Update()
    {
        UpdateSprite();
    }

    public void ChangeIngredient(IngredientData newData)
    {
        if (newData == null)
        {
            Debug.LogWarning("Tried to change ingredient into null data.");
            return;
        }

        _data = newData;
        IngredientInstance.ChangeData(newData);
        gameObject.name = newData.Name;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }

        if (_image == null)
        {
            _image = GetComponentInChildren<Image>();
        }

        if (_image == null)
        {
            Debug.LogWarning("No Image found on " + gameObject.name);
            return;
        }

        if (IngredientInstance == null || IngredientInstance.Data == null)
        {
            return;
        }

        if (IngredientInstance.IsSpoiled && IngredientInstance.Data.SpoiledSprite != null)
        {
            _image.sprite = IngredientInstance.Data.SpoiledSprite;
        }
        else
        {
            if(IngredientInstance.IsPlated)
            {
                _image.sprite = IngredientInstance.Data.PlatedSprite;
            }
            else
            {
                _image.sprite = IngredientInstance.Data.NormalSprite;
            }
        }
    }
}