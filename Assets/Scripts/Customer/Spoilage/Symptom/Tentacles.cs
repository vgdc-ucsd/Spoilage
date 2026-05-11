using UnityEngine;

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

