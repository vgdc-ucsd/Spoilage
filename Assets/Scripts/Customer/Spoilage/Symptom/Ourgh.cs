using UnityEngine;

public class Ourgh : AbstractSpoilageSymptom
{
    public Ourgh()
    {
        category = SpoilageCategory.DISGUST;
    }

    public override void ApplySpoilage() {
        Debug.Log("Ourgh");
    }
}

