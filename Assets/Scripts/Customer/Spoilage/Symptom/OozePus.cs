using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
public class OozePus : AbstractSpoilageSymptom
{
    private static string[] spritePaths = {
        "Art/Customers/Spoilage/spoilageSymptoms_pustules#1",
        "Art/Customers/Spoilage/spoilageSymptoms_pustules#2",
        "Art/Customers/Spoilage/spoilageSymptoms_pustules#3",
        "Art/Customers/Spoilage/spoilageSymptoms_pustules#4",
    };
    public OozePus()
    {
        category = SpoilageCategory.RAGE;
    }

    public override void ApplySpoilage() {
        Debug.Log("Ooze Pus");
        ApplyFrontStaticSprite(spritePaths);
    }
}
