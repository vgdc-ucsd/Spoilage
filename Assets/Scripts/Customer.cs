using System.Collections.Generic;
using UnityEngine;
using System;   

public class Customer : MonoBehaviour
{
    public CustomerData customerData;
    public GameObject customerObject;
    public int spoilage;
    
    public void InstantiateCustomer()
    {
        for (int i = 0; i < CustomerData.NUM_SPRITES; i++)
        {
            GameObject newSprite = new GameObject("Customer Sprite " + i);
            SpriteRenderer renderer = newSprite.AddComponent<SpriteRenderer>();
            renderer.sprite = customerData.sprites[i];
            newSprite.transform.position = customerData.spriteOffsets[i];
            newSprite.transform.SetParent(customerObject.transform);
            Instantiate(newSprite);
        }
    }
}