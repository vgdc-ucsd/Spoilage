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
        // Smaller = faster, gets applied to delays.
        _blinkingSpeedMultiplier = 0.25f;
        category = SpoilageCategory.DISTRESS;
    }

    public override void ApplySpoilage()
    {
        Debug.Log("Blinking Rapidly");
        SetBlinkMultiplier(_blinkingSpeedMultiplier);
        // TODO: VISUALS
    }
}
