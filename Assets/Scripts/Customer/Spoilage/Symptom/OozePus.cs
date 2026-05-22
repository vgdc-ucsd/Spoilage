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
    private Sprite sprite;
    public OozePus()
    {
        category = SpoilageCategory.RAGE;
    }

    public override void ApplySpoilage() {
        Debug.Log("Ooze Pus");
        
        string chosen = spritePaths[Random.Range(0, spritePaths.Length)];
        sprite = Resources.Load<Sprite>(chosen);

        customer.transform.Find("Sprites/SPOILAGE/SPOILAGE_FRONT_1").GetComponent<SpriteRenderer>().sprite = sprite;
        customer.transform.Find("Sprites/SPOILAGE/SPOILAGE_FRONT_2").GetComponent<SpriteRenderer>().sprite = sprite;
        customer.GetComponent<CustomerAnimation>().SetSpoilageStatus(CustomerAnimation.SpoilageStatus.FRAME_1);
    }
}

