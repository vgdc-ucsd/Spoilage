using UnityEngine;

public class Gurgling : AbstractSpoilageSymptom
{
    public Gurgling()
    {
        category = SpoilageCategory.HUNGER;
    }

    public override void ApplySpoilage() {
        Debug.Log("Gurgling");
    }
}

