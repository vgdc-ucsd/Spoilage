using UnityEngine;

public sealed class Ingredient
{
    public IngredientData Data { get; private set; }

    public float SpoilagePercent { get; private set; }

    public bool IsSpoiled => SpoilagePercent >= 100f;

    public float? SeasoningBonus { get; private set; }

    public bool IsSeasoned => SeasoningBonus.HasValue;

    private const float CONSTANT_SEASONING_BONUS = 10f;
    public Ingredient(IngredientData data)
    {
        Data = data;
        SpoilagePercent = 0f;
        SeasoningBonus = null;
    }

    public void ChangeData(IngredientData newData)
    {
        Data = newData;
        SeasoningBonus = null;
    }

    public bool Season()
    {
        if(IsSeasoned) return false;
        SeasoningBonus = CONSTANT_SEASONING_BONUS;
        return true;
    }

    public bool RemoveSeasoning()
    {
        if (!IsSeasoned) return false;
        SeasoningBonus = null;
        return true;
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