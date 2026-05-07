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
        if (_image == null || IngredientInstance == null) return;

        if (IngredientInstance.IsSpoiled)
        {
            _image.sprite = IngredientInstance.Data.SpoiledSprite;
        }
        else
        {
            _image.sprite = IngredientInstance.Data.NormalSprite;
        }
    }
}