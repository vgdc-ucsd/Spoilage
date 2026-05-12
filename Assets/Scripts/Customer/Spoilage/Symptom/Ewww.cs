using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
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

