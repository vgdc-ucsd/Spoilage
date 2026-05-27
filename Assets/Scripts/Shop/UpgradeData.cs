using UnityEngine;

public enum UpgradeType
{
    // --- Generic station effects (handled by CookingStation.ApplyUpgrade) ---
    CapacityIncrease,       // value: integer slots added
    SpeedBoost,             // value: fractional reduction (0.25 = 25% faster)
    QualityBonus,           // value: flat quality points added

    // --- Global restaurant effects (handled by UpgradeManager modifier getters) ---
    SpoilRateMultiplier,    // value: multiplier on spoil speed (2 = 2x faster, 0.5 = half)
    GuardPowerBoost,        // value: flat integer added to guard max presses
    DayDurationMultiplier,  // value: multiplier on day length (1.2 = 20% longer)
    InstantCooking,         // value: number of instant-cook uses granted
    PatienceDecayMultiplier,// value: multiplier on patience drain rate (0.75 = 25% slower)
    ReputationLimitIncrease,// value: flat increase to max reputation cap
    StreakBonus,            // value: perfect dishes required per 1.5x reward trigger
    FailMoneyReduction,     // value: fractional reduction to money lost on failed dishes
    PatienceTip,            // value: fraction of remaining patience paid as tip bonus
    SpoiledCustomerChance,  // value: delta applied to unspoiled threshold (negative = fewer spoiled)

    // --- Station-specific behaviours checked via UpgradeManager.HasUpgrade(id) ---
    Custom
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Shop/Upgrade")]
public class UpgradeData : ScriptableObject
{
    [Header("Identity")]
    public string upgradeID;
    public string upgradeName;
    public bool isTemporary;

    [Tooltip("Upgrade IDs that must be purchased before this one becomes available.")]
    public string[] prerequisites;

    [Header("Effect")]
    [Tooltip("Matches the _station field on CookingStation. Leave empty for global or Custom upgrades.")]
    public string targetStation;
    public UpgradeType upgradeType;
    [Tooltip("See UpgradeType comments for what value means per type.")]
    public float value;

    [Header("Shop Display")]
    public string description;
    public Sprite icon;
    public int price;
    public Color color = Color.white;
}
