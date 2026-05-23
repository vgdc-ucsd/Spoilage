using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
public class BugSkittering : AbstractSpoilageSymptom
{
    private static string[] spritePaths = {
        "Art/Customers/Spoilage/spoilageSymptoms_bugs#1",
        "Art/Customers/Spoilage/spoilageSymptoms_bugs#2",
        "Art/Customers/Spoilage/spoilageSymptoms_bugs#12-13",
    };

    public BugSkittering()
    {
        category = SpoilageCategory.TEMPERATURE;
    }

    public override void ApplySpoilage() {
        Debug.Log("Bug Skittering");
        ApplyFrontSpriteSheet(spritePaths);
        // TODO: AUDIO
    }
}
