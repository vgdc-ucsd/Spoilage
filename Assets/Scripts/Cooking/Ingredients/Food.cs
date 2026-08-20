using UnityEngine;

public class Food : Placeable
{
    public IngredientData Data { get; private set; }
    
    public bool IsOvercooked { get; private set; }
    public float SpoilagePercent { get; private set; }

    public bool IsSpoiled => SpoilagePercent >= 100f;
    public bool IsPlated;
    public float QualityPercent { get; set; }
    public float? SeasoningBonus { get; private set; }

    public bool IsSeasoned => SeasoningBonus.HasValue;

    private const float CONSTANT_QUALITY_SEASONING_BONUS = 10f;
    private const float CONSTANT_QUALITY_OVERCOOKED_DEDUCTION = 20f;

    public Food(IngredientData data)
    {
        Data = data;
        SpoilagePercent = 0f;
        QualityPercent = 0f;
        SeasoningBonus = null;
    }

    public Food(IngredientData data, float quality, float spoilage)
    {
        Data = data;
        SpoilagePercent = spoilage;
        QualityPercent = quality;
        SeasoningBonus = null;
    }

    public void ChangeData(IngredientData newData)
    {
        Data = newData;
        QualityPercent -= (SeasoningBonus ?? 0f);
        SeasoningBonus = null;
    }

    public bool Season()
    {
        if(IsSeasoned) return false;
        SeasoningBonus = CONSTANT_QUALITY_SEASONING_BONUS;
        QualityPercent += (SeasoningBonus ?? 0f);
        return true;
    }

    public bool RemoveSeasoning()
    {
        if (!IsSeasoned) return false;
        QualityPercent -= (SeasoningBonus ?? 0f);
        SeasoningBonus = null;
        return true;
    }

    public void SetOvercooked(bool state)
    {
        IsOvercooked = state;
        QualityPercent -= CONSTANT_QUALITY_OVERCOOKED_DEDUCTION;
    }

    public void AddSpoilagePercent(float amount)
    {
        SpoilagePercent = Mathf.Clamp(SpoilagePercent + amount, 0f, 100f);
    }

    public void SetSpoilagePercent(float percent)
    {
        SpoilagePercent = Mathf.Clamp(percent, 0f, 100f);
    }
}