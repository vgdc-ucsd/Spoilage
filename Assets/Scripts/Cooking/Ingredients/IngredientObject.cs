using UnityEngine;
using UnityEngine.UI;

public class IngredientObject : MonoBehaviour
{
    [SerializeField] private IngredientData _data;
    [SerializeField] private Image _image;
    [SerializeField] private Image _plateImage;
    [SerializeField] private GameObject _seasoning;

    public Food IngredientInstance { get; private set; }
    // public float GetQualityPercent => IngredientInstance.QualityPercent;
    public float QualityPercent { 
        get => IngredientInstance.QualityPercent; 
        set => IngredientInstance.QualityPercent = value; }


    public float GetSeasoningBonus => IngredientInstance?.SeasoningBonus ?? 0f;

    public bool IsSeasoned => IngredientInstance != null && IngredientInstance.IsSeasoned;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        IngredientInstance = new Food(_data);
        ChangeIngredient(_data);
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
        gameObject.name = newData.Name;
        IngredientInstance.ChangeData(newData);
        UpdateSprite();
    }

    private void UpdateSprite()
    {
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
            _image.sprite = IngredientInstance.Data.NormalSprite;
        }

        if (IngredientInstance.IsPlated && IngredientInstance.Data.PlateSprite != null)
        {
            _plateImage.sprite = IngredientInstance.Data.PlateSprite;
            _plateImage.enabled = true;
        }
        else
        {
            _plateImage.enabled = false;
        }

        if (IngredientInstance.Data.IsSmallIngredient)
        {
            _rectTransform.sizeDelta = new Vector2(100, 100);
        }
        else 
        {
            _rectTransform.sizeDelta = new Vector2(200, 200);
        }

        _seasoning.SetActive(IngredientInstance.IsSeasoned);
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