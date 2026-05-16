using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    public CustomerData customerData;
    
    public GameObject customerObject;
    public int spoilage;

    [SerializeField] private Image _spoilageFront;
    [SerializeField] private Image _spoilageBack;
    [SerializeField] private RectTransform _facialFeatures;

    [ContextMenu("Initialize Customer")]
    public void InitializeCustomer()
    {
        float scaleFactor = GetComponentInParent<Canvas>().scaleFactor;

        if (customerData == null)
        {
            customerData = CustomerManager.Instance.GenerateCustomerData();
        }

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
                Image img = currTransform.GetComponent<Image>(); 
                img.sprite = customerData.sprites[i];
                img.SetNativeSize();
            }
            /*
            switch ((CustomerData.Indexes)i)
            {
                case CustomerData.Indexes.MOUTH_OPEN:
                case CustomerData.Indexes.MOUTH_CLOSED:
                case CustomerData.Indexes.MOUTH_ANGER:
                case CustomerData.Indexes.MOUTH_DISGUST:
                    transform.Find("Sprites/FACIAL_FEATURES").localPosition = customerData.faceOffset;
                    transform.Find("Sprites/FACIAL_FEATURES").localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                case CustomerData.Indexes.EYES_OPEN:
                case CustomerData.Indexes.EYES_CLOSED:
                case CustomerData.Indexes.EYES_ANGER:
                case CustomerData.Indexes.EYES_DISGUST:
                case CustomerData.Indexes.EYES_WIDENING:
                    transform.Find("Sprites/FACIAL_FEATURES").localPosition = customerData.eyeOffset;
                    transform.Find("Sprites/FACIAL_FEATURES").localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                    // TODO: Use this location to apply the correct LOCAL offsets (the facial features that look right on its normal base model) (grab from CustomerManager/CustomerData list of local offsets)
                    //transform.Find("Sprites/FACIAL_FEATURES/" + ((CustomerData.Indexes)i).ToString()).localPosition = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    // TODO: once local positions are correct, apply GLOBAL offsets based on the different in height of character and face size (grab from CustomerManager/CustomerData list of global offsets)
            }
            */
        }

        if (customerData.spoilage == CustomerData.Spoilage.STAGE_I)
        {
            _spoilageFront.enabled = false;
            customerData.spoilageSymtomp = AbstractSpoilageSymptom.GenerateSymptom(this);
        }
        else if (customerData.spoilage == CustomerData.Spoilage.UNSPOILED)
        {
            _spoilageFront.enabled = false;
            _spoilageBack.enabled = false;
        }

        _facialFeatures.localPosition = customerData.faceOffset * scaleFactor;
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
