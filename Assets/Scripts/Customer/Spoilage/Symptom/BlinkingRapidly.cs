using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
public class BlinkingRapidly : AbstractSpoilageSymptom
{
    private float _blinkingSpeedMultiplier;

    public BlinkingRapidly()
    {
        _blinkingSpeedMultiplier = 1;
        category = SpoilageCategory.DISTRESS;
    }

    public override void ApplySpoilage()
    {
        Debug.Log("Blinking Rapidly");
    }
}
