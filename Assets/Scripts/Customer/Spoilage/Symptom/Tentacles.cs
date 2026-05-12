using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
public class Tentacles : AbstractSpoilageSymptom
{
    public Tentacles()
    {
        category = SpoilageCategory.HUNGER;
    }

    public override void ApplySpoilage() {
        Debug.Log("Tentacles");
    }
}

