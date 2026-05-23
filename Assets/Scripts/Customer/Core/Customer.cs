using UnityEngine;
using UnityEngine.InputSystem;

public class Customer : MonoBehaviour
{
    public CustomerData customerData;
    
    public GameObject customerObject;

    private const string SK_PROPHET = "a";
    private const string SK_BILLMAN = "b";    
    private const string SK_SISTER = "c";    
    private const string SK_PALE = "p";    
    private const string SK_DRUNK = "d";    
    private const string SK_WIDOW = "e";    
    private const string SK_DOCTOR = "f";    
    private const string SK_VIOLENT = "g";    
    private const string SK_DEAF = "h";    
    private const string SK_FAMISHED = "i";    
    private const string SK_EXECUTOR = "j";    

    [ContextMenu("Initialize Customer")]
    public void InitializeCustomer()
    {
        
        if (customerData == null)
        {
            customerData = CustomerManager.Instance.GenerateCustomerData();
        }
/*
        if (customerData.spoilage >= CustomerData.Spoilage.STAGE_I
            && customerData.spoilageSymptom == null)
        {
            customerData.spoilageSymptom = CustomerManager.GenerateSymptom();
        }

        if (customerData.spoilageSymptom != null)
        {
            customerData.spoilageSymptom.AssignCustomer(gameObject);
            customerData.spoilageSymptom.Register();

            // DEBUG
            //customerData.spoilageSymptom.ApplySpoilage(); 
        }
*/

        for (int i = 0; i < customerData.sprites.Length; i++)
        {
            Transform currTransform;
            switch ((CustomerData.Indexes)i)
            {
                case CustomerData.Indexes.MOUTH_OPEN:
                case CustomerData.Indexes.MOUTH_CLOSED:
                case CustomerData.Indexes.MOUTH_DISGUST:
                case CustomerData.Indexes.MOUTH_ANGER:
                case CustomerData.Indexes.EYES_OPEN:
                case CustomerData.Indexes.EYES_CLOSED:
                case CustomerData.Indexes.EYES_DISGUST:
                case CustomerData.Indexes.EYES_ANGER:
                case CustomerData.Indexes.EYES_WIDENING:
                    currTransform = transform.Find("Sprites/FACIAL_FEATURES/" + ((CustomerData.Indexes)i).ToString());
                    break;
                default:
                    currTransform = transform.Find("Sprites/" + ((CustomerData.Indexes)i).ToString());
                    break;
            }
            if (currTransform != null)
            {
                currTransform.GetComponent<SpriteRenderer>().sprite = customerData.sprites[i];
            }
        }

        if (customerData.tier == CustomerData.Tier.None)
        {
            if (customerData.spoilage == CustomerData.Spoilage.STAGE_I)
            {
                customerData.spoilageSymptom.AssignCustomer(transform.gameObject);
                customerData.spoilageSymptom.Register();
            } 
            else if (customerData.spoilage == CustomerData.Spoilage.STAGE_II)
            {
                transform.Find("Sprites/SPOILAGE/SPOILAGE_BACK_1").GetComponent<SpriteRenderer>().sprite = 
                    customerData.sprites[(int) CustomerData.Indexes.TENDRILS_1];

                transform.Find("Sprites/SPOILAGE/SPOILAGE_BACK_2").GetComponent<SpriteRenderer>().sprite = 
                    customerData.sprites[(int) CustomerData.Indexes.TENDRILS_2];

                GetComponent<CustomerAnimation>().StartSpoilageAnim();
            }
        } 
        else // Key or semi-key
        {
            PlayerData player = SaveManager.Instance.Player;
            switch (customerData.id)
            {
                case SK_PROPHET:
                    // Unknown if special behavior is needed
                    break;
                case SK_BILLMAN:
                    // if day > ?? assign customer to spoilage
                    // else set customerData.spoilageSymptom to null
                    break;
                case SK_SISTER:
                    // if day > ?? assign customer to spoilage
                    // else set customerData.spoilageSymptom to null
                    break;
                case SK_DRUNK:
                    // if day > ?? assign customer to spoilage
                    // else set customerData.spoilageSymptom to null
                    break;
                case SK_WIDOW:
                    // Assign customer to spoilage
                    break;
                case SK_DOCTOR:
                    // if day < ?? assign customer to spoilage
                    // else load and apply special sprites
                    break;
                
                case SK_PALE:
                case SK_VIOLENT:
                case SK_DEAF:
                case SK_FAMISHED:
                case SK_EXECUTOR:
                    // Start spoilage anim?
                    break;
            }
        }
        

        // Apply offsets
        transform.Find("Sprites/FACIAL_FEATURES").localPosition = customerData.faceOffset;
        transform.Find("Sprites/SPOILAGE").localPosition = customerData.faceOffset;
    }

    private void OnDestroy()
    {
        if (customerData != null && customerData.spoilageSymptom != null)
        {
            customerData.spoilageSymptom.Unregister();
        }
    }
    
    // public void InstantiateCustomer()
    // {
    //     for (int i = 0; i < CustomerData.NUM_SPRITES; i++)
    //     {
    //         GameObject newSprite = new GameObject("Customer Sprite " + i);
    //         SpriteRenderer renderer = newSprite.AddComponent<SpriteRenderer>();
    //         renderer.sprite = customerData.sprites[i];
    //         newSprite.transform.position = customerData.spriteOffsets[i];
    //         newSprite.transform.SetParent(customerObject.transform);
    //         Instantiate(newSprite);
    //     }
    // }
}
