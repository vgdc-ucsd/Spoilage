using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
public class Threat : AbstractSpoilageSymptom
{
    public Threat()
    {
        category = SpoilageCategory.RAGE;
    }

    public override void ApplySpoilage() {
        Debug.Log("Threat");
    }
}

