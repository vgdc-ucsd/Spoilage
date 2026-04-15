using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public CustomerData generateCustomerData()
    {
        CustomerData newData = new CustomerData
        {
            spoilage = Random.Range(0, 1.0f),
            sprites = new Sprite[CustomerData.NUM_SPRITES],
            spriteOffsets = new Vector3[CustomerData.NUM_SPRITES]
        };

        return newData;
    }

}