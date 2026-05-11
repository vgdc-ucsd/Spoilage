using UnityEngine;

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

