using UnityEngine;
using UnityEngine.InputSystem;

public class Customer : MonoBehaviour
{
    public CustomerData customerData;
    public Vector3 faceOffset = new Vector3(0, 1.4f, 0);
    public Vector3 eyeOffset = new Vector3(0, 2.0f, 0);

    public void Initialize()
    {
        GameObject spriteHolder = new GameObject("Sprites");
        spriteHolder.transform.parent = transform;
        GameObject facialFeaturesHolder = new GameObject("FACIAL_FEATURES");
        facialFeaturesHolder.transform.parent = spriteHolder.transform;
        Debug.Log("Face offset: " + faceOffset.ToString());
        for (int i = 0; i < customerData.sprites.Length; i++)
        {
            if (customerData.sprites[i] != null)
            {
                GameObject newSprite = new GameObject(((CustomerData.Indexes)i).ToString());
                newSprite.AddComponent<SpriteRenderer>();
                newSprite.GetComponent<SpriteRenderer>().sprite = customerData.sprites[i];
                switch ((CustomerData.Indexes)i)
                {
                    case CustomerData.Indexes.EYES_OPEN:
                        newSprite.transform.parent = facialFeaturesHolder.transform;
                        newSprite.transform.Translate(new Vector3(0.0f, 0.0f, -0.01f * i) + eyeOffset);
                        break;
                    case CustomerData.Indexes.MOUTH_CLOSED:
                        newSprite.transform.parent = facialFeaturesHolder.transform;
                        newSprite.transform.Translate(new Vector3(0.0f, 0.0f, -0.01f * i) + faceOffset);
                        break;
                    case CustomerData.Indexes.EYES_CLOSED:
                    case CustomerData.Indexes.EYES_ANGER:
                    case CustomerData.Indexes.EYES_DISGUST:
                    case CustomerData.Indexes.EYES_WIDENING:
                        newSprite.transform.parent = facialFeaturesHolder.transform;
                        newSprite.transform.Translate(new Vector3(0.0f, 0.0f, -0.01f * i) + eyeOffset);
                        newSprite.SetActive(false);
                        break;
                    case CustomerData.Indexes.MOUTH_OPEN:
                    case CustomerData.Indexes.MOUTH_ANGER:
                    case CustomerData.Indexes.MOUTH_DISGUST:
                        newSprite.transform.parent = facialFeaturesHolder.transform;
                        newSprite.transform.Translate(new Vector3(0.0f, 0.0f, -0.01f * i) + faceOffset);
                        newSprite.SetActive(false);
                        break;
                    default:
                        newSprite.transform.parent = spriteHolder.transform;
                        newSprite.transform.Translate(new Vector3(0.0f, 0.0f, -0.01f * i));
                        break;
                }
            }
        }
    }

    [ContextMenu("Generate Customer")]
    public void GenerateCustomer()
    {
        if (customerData == null) //(customerData == null) DEBUG PLEASE UNDO COMMENT LATER
        {
            Debug.Log("Generating new customer");
            customerData = CustomerManager.GenerateCustomerData();
        }
        else
        {
            Debug.Log("Using existing customer");
        }

        for (int i = 0; i < CustomerData.NUM_SPRITES; i++)
        {
            Transform curTransform;
            switch ((CustomerData.Indexes)i)
            {
                case CustomerData.Indexes.MOUTH_OPEN:
                case CustomerData.Indexes.MOUTH_CLOSED:
                case CustomerData.Indexes.MOUTH_ANGER:
                case CustomerData.Indexes.MOUTH_DISGUST:
                case CustomerData.Indexes.EYES_OPEN:
                case CustomerData.Indexes.EYES_CLOSED:
                case CustomerData.Indexes.EYES_ANGER:
                case CustomerData.Indexes.EYES_DISGUST:
                case CustomerData.Indexes.EYES_WIDENING:
                    curTransform = transform.Find("Sprites/FACIAL_FEATURES/" + ((CustomerData.Indexes)i).ToString());

                    break;
                default:
                    curTransform = transform.Find("Sprites/" + ((CustomerData.Indexes)i).ToString());
                    break;
            }
            if (curTransform != null)
            {
                curTransform.GetComponent<SpriteRenderer>().sprite = customerData.sprites[i];
            }
            /*
            switch ((CustomerData.Indexes)i)
            {
                case CustomerData.Indexes.MOUTH_OPEN:
                case CustomerData.Indexes.MOUTH_CLOSED:
                case CustomerData.Indexes.MOUTH_ANGER:
                case CustomerData.Indexes.MOUTH_DISGUST:
                case CustomerData.Indexes.EYES_OPEN:
                case CustomerData.Indexes.EYES_CLOSED:
                case CustomerData.Indexes.EYES_ANGER:
                case CustomerData.Indexes.EYES_DISGUST:
                case CustomerData.Indexes.EYES_WIDENING:
                    // TODO: Use this location to apply the correct LOCAL offsets (the facial features that look right on its normal base model) (grab from CustomerManager/CustomerData list of local offsets)
                    //transform.Find("Sprites/FACIAL_FEATURES/" + ((CustomerData.Indexes)i).ToString()).localPosition = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    // TODO: once local positions are correct, apply GLOBAL offsets based on the different in height of character and face size (grab from CustomerManager/CustomerData list of global offsets)
                    //transform.Find("Sprites/FACIAL_FEATURES").localPosition = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    //transform.Find("Sprites/FACIAL_FEATURES").localScale = new UnityEngine.Vector3(1.0f, 1.0f, 1.0f);
                    break;
            }
            */
        }
        // APPLY OFFSET
        Debug.Log("Applying Offset");
        transform.Find("Sprites/FACIAL_FEATURES").position = customerData.faceOffset;
    }
}