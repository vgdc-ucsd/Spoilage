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

    private Sprite[] sprites;
    
    public Sweating()
    {
        category = SpoilageCategory.TEMPERATURE;
    }

    public override void ApplySpoilage() {
        Debug.Log("Sweating");
        
        string chosen = spritePaths[Random.Range(0, spritePaths.Length)];
        sprites = Resources.LoadAll<Sprite>(chosen);

        customer.transform.Find("Sprites/SPOILAGE/SPOILAGE_FRONT_1").GetComponent<SpriteRenderer>().sprite = sprites[0];
        customer.transform.Find("Sprites/SPOILAGE/SPOILAGE_FRONT_2").GetComponent<SpriteRenderer>().sprite = sprites[1];

        customer.GetComponent<CustomerAnimation>().StartSpoilageAnim();

        // TODO: VISUALS
    }
}
