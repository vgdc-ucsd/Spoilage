using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
public class OozePus : AbstractSpoilageSymptom
{
    public OozePus()
    {
        category = SpoilageCategory.RAGE;
    }

    public override void ApplySpoilage() {
        Debug.Log("Ooze Pus");
    }
}

