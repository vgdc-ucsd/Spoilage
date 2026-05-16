using UnityEngine;
using UnityEngine.UI;

public class IngredientObject : MonoBehaviour
{
    [SerializeField] private IngredientData _data;
    [SerializeField] private Image _image;

    public Ingredient IngredientInstance { get; private set; }

    public float GetSeasoningBonus => IngredientInstance?.SeasoningBonus ?? 0f;

    public bool IsSeasoned => IngredientInstance != null && IngredientInstance.IsSeasoned;

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

        // keep the quality data so it stacks up
        float oldQuality = _data.QualityPercent;
        newData.QualityPercent = oldQuality;
        
        _data = newData;
        gameObject.name = newData.Name;
        IngredientInstance.ChangeData(newData);
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

    public bool SeasonIngredient()
    {
        if (IngredientInstance == null) return false;
        return IngredientInstance.Season();
    }

    public bool RemoveSeasoning()
    {
        if (IngredientInstance == null) return false;
        return IngredientInstance.RemoveSeasoning();
    }
}