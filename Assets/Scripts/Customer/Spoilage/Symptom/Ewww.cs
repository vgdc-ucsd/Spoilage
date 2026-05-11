using UnityEngine;

public class Ewww : AbstractSpoilageSymptom
{
    public Ewww()
    {
        category = SpoilageCategory.DISGUST;
    }

    public override void ApplySpoilage() {
        Debug.Log("Ewww");
    }
}

