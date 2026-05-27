using UnityEngine;

public enum UpgradeType
{
    CapacityIncrease,
    SpeedBoost,
    QualityBonus
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Shop/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeID;
    public string upgradeName;

    [Tooltip("Matches the _station field on CookingStation. Leave empty to apply to all stations.")]
    public string targetStation;

    public UpgradeType upgradeType;

    [Tooltip("CapacityIncrease: integer slots added. SpeedBoost: fractional reduction (0.25 = 25% faster). QualityBonus: flat points added (0-100 scale).")]
    public float value;

    [Header("Shop Display")]
    public Sprite icon;
    public int price;
    public Color color = Color.white;
}
