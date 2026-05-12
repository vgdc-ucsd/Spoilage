using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
public class BugSkittering : AbstractSpoilageSymptom
{
    public BugSkittering()
    {
        category = SpoilageCategory.TEMPERATURE;
    }

    public override void ApplySpoilage() {
        Debug.Log("Bug Skittering");
    }
}
