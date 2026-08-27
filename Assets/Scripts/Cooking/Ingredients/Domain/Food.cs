using UnityEngine;

public class Food : Placeable
{
    public IngredientData Data { get; private set; }
    
    public bool IsOvercooked { get; private set; }
    public float SpoilagePercent { get; private set; }

    public bool Spoiling => SpoilagePercent > 0f && SpoilagePercent < 1f;
    public bool IsSpoiled => SpoilagePercent >= 1f;
    public bool IsPlated;
    public float QualityPercent { get; set; }
    public float? SeasoningBonus { get; private set; }

    public bool IsSeasoned => SeasoningBonus.HasValue;
    public override PlaceableUI UI => _ui;

    private FoodUI _ui;
    private float _timer = 0f;

    private const float CONSTANT_QUALITY_SEASONING_BONUS = 10f;
    private const float CONSTANT_QUALITY_OVERCOOKED_DEDUCTION = 20f;
    private const float SPOIL_TIME = 15f;

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
        _timer = spoilage * SPOIL_TIME;
    }

    public void SetUI(FoodUI ui)
    {
        _ui = ui;
        _ui.SetFood(this);
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

    public void Spoil(float dt)
    {
        _timer += dt;
        
        bool becomeSpoiled = IsSpoiled;
        bool becomeSpoiling = Spoiling;

        SpoilagePercent = Mathf.Clamp01(_timer / SPOIL_TIME);
        
        if (!IsSpoiled) _ui.SetSpoilage(SpoilagePercent);
        
        becomeSpoiled = !becomeSpoiled && IsSpoiled;
        becomeSpoiling = !becomeSpoiling && Spoiling;

        if (becomeSpoiled) _ui.Spoil();
        if (becomeSpoiling) _ui.ShowTimer(true);
    }
}