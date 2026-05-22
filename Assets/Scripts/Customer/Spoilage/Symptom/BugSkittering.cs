using System.Collections;
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

    private Sprite[] sprites;
    public BugSkittering()
    {
        category = SpoilageCategory.TEMPERATURE;
    }

    public override void ApplySpoilage() {
        Debug.Log("Bug Skittering");
        string chosen = spritePaths[Random.Range(0, spritePaths.Length)];
        sprites = Resources.LoadAll<Sprite>(chosen);

        customer.transform.Find("Sprites/SPOILAGE/SPOILAGE_FRONT_1").GetComponent<SpriteRenderer>().sprite = sprites[0];
        customer.transform.Find("Sprites/SPOILAGE/SPOILAGE_FRONT_2").GetComponent<SpriteRenderer>().sprite = sprites[1];

        customer.GetComponent<CustomerAnimation>().StartSpoilageAnim();
        // TODO: AUDIO
    }

    // public void OnEnable()
    // {
    //     string chosen = spritePaths[Random.Range(0, spritePaths.Length)];
    //     Debug.Log("Chosen Path: " + chosen);
    //     sprites = Resources.LoadAll<Sprite>(chosen);
    // }
}
