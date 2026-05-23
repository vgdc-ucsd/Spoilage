using UnityEngine;
using UnityEngine.InputSystem.Interactions;

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

    private Sprite[] sprites;
    

    public Tentacles()
    {
        category = SpoilageCategory.HUNGER;
    }

    public override void ApplySpoilage() {
        Debug.Log("Tentacles");

        float percentBackOptions = (float) backSpritePaths.Length / (backSpritePaths.Length + frontSpritePaths.Length);
        Debug.Log(percentBackOptions);

        string chosen;
        bool back;

        if (Random.Range(0, 1f) <= percentBackOptions)
        {
            back = true;
            chosen = backSpritePaths[Random.Range(0, backSpritePaths.Length)];
        } else
        {
            back = false;
            chosen = frontSpritePaths[Random.Range(0, frontSpritePaths.Length)];
        }

        sprites = Resources.LoadAll<Sprite>(chosen);

        if (back)
        {
            Customer.SetSprite(customer, "Sprites/SPOILAGE/SPOILAGE_BACK_1", sprites[0]);
            Customer.SetSprite(customer, "Sprites/SPOILAGE/SPOILAGE_BACK_2", sprites[1]);
        } else
        {
            Customer.SetSprite(customer, "Sprites/SPOILAGE/SPOILAGE_FRONT_1", sprites[0]);
            Customer.SetSprite(customer, "Sprites/SPOILAGE/SPOILAGE_FRONT_2", sprites[1]);
        }
        
        customer.GetComponent<CustomerAnimation>().StartSpoilageAnim();
       
    }
}

