using UnityEngine;

public sealed class Ingredient
{
    public IngredientData Data { get; private set; }
    
    public bool IsOvercooked { get; private set; }
    public float SpoilagePercent { get; private set; }

    public bool IsSpoiled => SpoilagePercent >= 100f;
    public bool IsPlated;

    public Ingredient(IngredientData data)
    {
        Data = data;
        SpoilagePercent = 0f;
    }

    public void ChangeData(IngredientData newData)
    {
        Data = newData;
    }

    public void SetOvercooked(bool state)
    {
        IsOvercooked = state;
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