using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
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

