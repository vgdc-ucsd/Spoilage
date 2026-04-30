using UnityEngine;
using UnityEngine.InputSystem;

public class Customer : MonoBehaviour
{
    public CustomerData customerData;
    [ContextMenu("Generate Customer")]
    public void GenerateCustomer()
    {
        if (true) //(customerData == null) DEBUG PLEASE UNDO COMMENT LATER
        {
            Debug.Log("Generating new customer");
            customerData = CustomerManager.GenerateCustomerData();
        }
        else
        {
            Debug.Log("Using existing customer");
        }

        for (int i = 0; i < customerData.sprites.Length; i++)
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

        if (customerData.spoilage == CustomerData.Spoilage.SLIGHTLY)
        {
            transform.Find("Sprites/SPOILAGE_FRONT").GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (customerData.spoilage == CustomerData.Spoilage.NOT)
        {
            transform.Find("Sprites/SPOILAGE_FRONT").GetComponent<SpriteRenderer>().enabled = false;
            transform.Find("Sprites/SPOILAGE_BACK").GetComponent<SpriteRenderer>().enabled = false;
        }

        transform.Find("Sprites/FACIAL_FEATURES").position = customerData.faceOffset;
    }
}