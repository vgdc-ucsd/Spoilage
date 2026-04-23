using UnityEngine;

public class Customer : MonoBehaviour
{
    public CustomerData customerData;

    [ContextMenu("Generate Customer")]
    public void GenerateCustomer()
    {
        if (customerData == null)
        {
            Debug.Log("Generating new customer");
            customerData = CustomerManager.GenerateCustomerData();
        }
        else
        {
            Debug.Log("Using existing customer");
        }

        // TODO: Be sure to add the new indexes into the offset and sprite handling
        for (int i = 0; i < CustomerData.NUM_SPRITES; i++)
        {
            Transform curTransform;
            switch ((CustomerData.Indexes)i)
            {
                case CustomerData.Indexes.MOUTH_OPEN:
                case CustomerData.Indexes.MOUTH_CLOSED:
                case CustomerData.Indexes.EYES_OPEN:
                case CustomerData.Indexes.EYES_CLOSED:
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

            switch ((CustomerData.Indexes)i)
            {
                case CustomerData.Indexes.MOUTH_OPEN:
                case CustomerData.Indexes.MOUTH_CLOSED:
                case CustomerData.Indexes.EYES_OPEN:
                case CustomerData.Indexes.EYES_CLOSED:
                    // TODO: Use this location to apply the correct LOCAL offsets (the facial features that look right on its normal base model) (grab from CustomerManager/CustomerData list of local offsets)
                    //transform.Find("Sprites/FACIAL_FEATURES/" + ((CustomerData.Indexes)i).ToString()).localPosition = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    // TODO: once local positions are correct, apply GLOBAL offsets based on the different in height of character and face size (grab from CustomerManager/CustomerData list of global offsets)
                    //transform.Find("Sprites/FACIAL_FEATURES").localPosition = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    //transform.Find("Sprites/FACIAL_FEATURES").localScale = new UnityEngine.Vector3(1.0f, 1.0f, 1.0f);
                    break;
            }
        }
    }
}