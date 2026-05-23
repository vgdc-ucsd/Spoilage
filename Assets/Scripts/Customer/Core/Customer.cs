using UnityEngine;
using UnityEngine.InputSystem;

public class Customer : MonoBehaviour
{
    public CustomerData customerData;
    
    public GameObject customerObject;

    private const string SK_PROPHET = "Prophet";
    private const string SK_BILLMAN = "The Billman";    
    private const string SK_SISTER = "Unlucky Sister";
    private const string SK_PALE = "The Pale Spoiled";    
    private const string SK_DRUNK = "Drunk Beggar";    
    private const string SK_WIDOW = "Suspicious Widow";    
    private const string SK_DOCTOR = "Spoilage Doctor";    
    private const string SK_VIOLENT = "The Violen Spoiled";    
    private const string SK_DEFEATED = "The Defeated Spoiled";    
    private const string SK_FAMISHED = "The Famished Spoiled";    
    private const string SK_EXECUTOR = "Executor";    

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
            PlayerData p = SaveManager.Instance.Player;
            switch (customerData.id)
            {
                case SK_PROPHET:
                    // Unknown if special behavior is needed
                    break;
                case SK_BILLMAN:
                    applySpoilageOnDay(p, 24);
                    break;
                case SK_SISTER:
                    applySpoilageOnDay(p, 22);
                    break;
                case SK_DRUNK:
                    applySpoilageOnDay(p, 18);
                    break;
                case SK_WIDOW:
                    // Discrepancy between spreadsheet and coggle, spreadsheet
                    // says widow should start and end with stage I spoilage, 
                    // coggle says she should transition from unspoiled --> stage I on day 14
                    applySpoilageOnDay(p, 0); 
                    // applySpoilageOnDay(p, 14);
                    break;
                case SK_DOCTOR:
                    // if day < 21 assign customer to spoilage
                    // else load and apply special sprites
                    if (p.Day < 21)
                    {
                        customerData.spoilageSymptom.AssignCustomer(transform.gameObject);
                        customerData.spoilageSymptom.Register();
                    } else
                    {
                        customerData.spoilageSymptom.DeleteSymptom();
                        customerData.spoilageSymptom = null;

                        customerData.spoilage = CustomerData.Spoilage.STAGE_II;

                        // Apply stage II spoilage sprites
                        Sprite spoiledBase = Resources.Load<Sprite>("Art/Customers/Spoilage/Fully Spoiled/spoilageSymptoms_fullySpoiled_body1");
                        Sprite[] tendrils = Resources.LoadAll<Sprite>("Art/Customers/Spoilage/Fully Spoiled/spoilageSymptoms_fullySpoiled_tendrils");

                        transform.Find("Sprites/BODY").GetComponent<SpriteRenderer>().sprite = spoiledBase;

                        transform.Find("Sprites/SPOILAGE/SPOILAGE_BACK_1").GetComponent<SpriteRenderer>().sprite = tendrils[0];
                        transform.Find("Sprites/SPOILAGE/SPOILAGE_BACK_2").GetComponent<SpriteRenderer>().sprite = tendrils[1];
                        goto case SK_EXECUTOR;
                    }
                    break;
                
                case SK_PALE:
                case SK_VIOLENT:
                case SK_DEFEATED:
                case SK_FAMISHED:
                case SK_EXECUTOR:
                    GetComponent<CustomerAnimation>().StartSpoilageAnim();
                    break;
            }
        }
        

        // Apply offsets
        transform.Find("Sprites/FACIAL_FEATURES").localPosition = customerData.faceOffset;
        transform.Find("Sprites/SPOILAGE").localPosition = customerData.faceOffset;
    }

    private void applySpoilageOnDay(PlayerData player, int day)
    {
        if (player.Day >= day)
        {
            customerData.spoilageSymptom.AssignCustomer(transform.gameObject);
            customerData.spoilageSymptom.Register();
        }
        else
        {
            customerData.spoilageSymptom.DeleteSymptom();
            customerData.spoilageSymptom = null;
        }
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
