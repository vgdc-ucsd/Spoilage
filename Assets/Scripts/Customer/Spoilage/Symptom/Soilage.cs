using UnityEngine;

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
