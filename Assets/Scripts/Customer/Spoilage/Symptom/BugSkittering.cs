using UnityEngine;

public class BugSkittering : AbstractSpoilageSymptom
{
    public BugSkittering()
    {
        category = SpoilageCategory.TEMPERATURE;
    }

    public override void ApplySpoilage() {
        Debug.Log("Bug Skittering");
    }
}
