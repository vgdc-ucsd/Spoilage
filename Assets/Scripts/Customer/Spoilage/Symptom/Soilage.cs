using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
public class Soilage : AbstractSpoilageSymptom
{
    public Soilage()
    {
        category = SpoilageCategory.DISTRESS;
    }

    public override void ApplySpoilage() {
        Debug.Log("Soilage");
    }
}
