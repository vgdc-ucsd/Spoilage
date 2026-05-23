using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
public class Tentacles : AbstractSpoilageSymptom
{
    private static string[] backSpritePaths = 
    {
        "Art/Customers/Spoilage/spoilageSymptoms_tendrils#1",
    };

    private static string[] frontSpritePaths =
    {
        "Art/Customers/Spoilage/spoilageSymptoms_tendrils#2",
        "Art/Customers/Spoilage/spoilageSymptoms_tendrils#3",
    };

    public Tentacles()
    {
        category = SpoilageCategory.HUNGER;
    }

    public override void ApplySpoilage() {
        Debug.Log("Tentacles");

        int backCount = backSpritePaths.Length;
        int frontCount = frontSpritePaths.Length;
        bool useBack = Random.Range(0, backCount + frontCount) < backCount;
        Sprite[] sprites = LoadRandomSpriteSheet(useBack ? backSpritePaths : frontSpritePaths);

        if (useBack)
        {
            SetBackSprites(sprites);
        }
        else
        {
            SetFrontSprites(sprites);
        }

        StartSpoilageAnimation();
    }
}
