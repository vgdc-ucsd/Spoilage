using UnityEngine;

public class Sweating : AbstractSpoilageSymptom
{
    public Sweating()
    {
        category = SpoilageCategory.TEMPERATURE;
    }

    public override void ApplySpoilage() {
        Debug.Log("Sweating");
    }
}
