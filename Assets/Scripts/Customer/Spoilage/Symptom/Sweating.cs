using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
public class Sweating : AbstractSpoilageSymptom
{
    private static string[] spritePaths = {
        "Art/Customers/Spoilage/spoilageSymptoms_sweat#3",
        "Art/Customers/Spoilage/spoilageSymptoms_sweat#4",
        "Art/Customers/Spoilage/spoilageSymptoms_sweat#5",
        "Art/Customers/Spoilage/Sweat1_spritesheet",
        "Art/Customers/Spoilage/Sweat2_spritesheet",
    };

    public Sweating()
    {
        category = SpoilageCategory.TEMPERATURE;
    }

    public override void ApplySpoilage() {
        Debug.Log("Sweating");
        ApplyFrontSpriteSheet(spritePaths);
        // TODO: VISUALS
    }
}
