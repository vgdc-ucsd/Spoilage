using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    public CustomerData customerData;

    public GameObject customerObject;
    public CustomerDialogue Dialogue => _customerDialogue;
    public CustomerMovement Movement => _movement;

    private PlayerData Player => SaveManager.Instance.Player;
    private CustomerDialogue _customerDialogue;
    [SerializeField] private CustomerMovement _movement;

    private const string SK_PROPHET = "Prophet";
    private const string SK_BILLMAN = "Billman";
    private const string SK_SISTER = "Unlucky Twin Girl";
    private const string SK_PALE = "Pale Spoiled";
    private const string SK_DRUNK = "Drunk";
    private const string SK_WIDOW = "Suspicious Widow";
    private const string SK_DOCTOR = "Doctor";
    private const string SK_VIOLENT = "Violent Spoiled";
    private const string SK_DEFEATED = "Defeated Spoiled";
    private const string SK_FAMISHED = "Famished Spoiled";
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
                Image image = currTransform.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = customerData.sprites[i];
                    image.enabled = image.sprite != null;
                    image.SetNativeSize();
                }
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
            switch (customerData.id)
            {
                case SK_PROPHET:
                    // Unknown if special behavior is needed
                    break;
                case SK_BILLMAN:
                    applySpoilageOnDay(24);
                    break;
                case SK_SISTER:
                    applySpoilageOnDay(22);
                    break;
                case SK_DRUNK:
                    applySpoilageOnDay(18);
                    break;
                case SK_WIDOW:
                    // Discrepancy between spreadsheet and coggle, spreadsheet
                    // says widow should start and end with stage I spoilage,
                    // coggle says she should transition from unspoiled --> stage I on day 14
                    applySpoilageOnDay(0);
                    // applySpoilageOnDay(p, 14);
                    break;
                case SK_DOCTOR:
                    // if day < 21 assign customer to spoilage
                    // else load and apply special sprites
                    if (Player.Day < 21)
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
                        Sprite[] tendrils = Resources.LoadAll<Sprite>("Art/Customers/Spoilage/Fully Spoiled/spoilageSymptoms_fullySpoiled_tendrils_spritesheet");

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

        SetAnchoredPosition("Sprites/FACIAL_FEATURES", customerData.faceOffset);
        SetAnchoredPosition("Sprites/SPOILAGE", customerData.faceOffset);
    }

    public void SetDialogue(CustomerDialogue dialogue)
    {
        _customerDialogue = dialogue;        
    }

    private void applySpoilageOnDay(int day)
    {
        if (Player.Day >= day)
        {
            customerData.spoilageSymptom.AssignCustomer(transform.gameObject);
            customerData.spoilageSymptom.Register();
            customerData.spoilage = CustomerData.Spoilage.STAGE_I;
        }
        else
        {
            customerData.spoilageSymptom.DeleteSymptom();
            customerData.spoilageSymptom = null;
        }
    }

    public static void SetSprite(GameObject root, string path, Sprite sprite)
    {
        if (root == null) return;

        Transform slot = root.transform.Find(path);
        Image image = slot == null ? null : slot.GetComponent<Image>();
        if (image == null) return;

        image.sprite = sprite;
        image.enabled = sprite != null;
        image.SetNativeSize();
    }

    private void SetAnchoredPosition(string path, Vector3 position)
    {
        RectTransform rectTransform = transform.Find(path) as RectTransform;
        if (rectTransform == null) return;

        rectTransform.anchoredPosition = new Vector2(position.x, position.y);
    }

    private void OnDestroy()
    {
        if (customerData != null && customerData.spoilageSymptom != null)
        {
            customerData.spoilageSymptom.Unregister();
        }
    }
    
}
