using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    public CustomerData customerData;
    
    public GameObject customerObject;
    //public int spoilage;

    [ContextMenu("Initialize Customer")]
    public void InitializeCustomer()
    {
        
        if (customerData == null)
        {
            customerData = CustomerManager.Instance.GenerateCustomerData();
        }

        if (customerData.spoilageSymptom != null && customerData.spoilageSymptom.customer == null)
        {
            customerData.spoilageSymptom.customer = transform.gameObject;

            // DEBUG
            //customerData.spoilageSymptom.ApplySpoilage(); 
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
                Image image = currTransform.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = customerData.sprites[i];
                    image.enabled = image.sprite != null;
                    image.SetNativeSize();
                }
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

        SetAnchoredPosition("Sprites/FACIAL_FEATURES", customerData.faceOffset);
        SetAnchoredPosition("Sprites/SPOILAGE", customerData.faceOffset);

        
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
    
}
