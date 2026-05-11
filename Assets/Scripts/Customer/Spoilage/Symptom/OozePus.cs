using UnityEngine;

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

