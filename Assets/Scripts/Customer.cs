using System.Collections.Generic;
using UnityEngine;
using System;   

public class Customer
{
    public CustomerData customerData;
    public int spoilage;
    
    public void InstantiateCustomer()
    {
        GameObject customerObject = new GameObject("Customer");
        for (int i = 0; i < CustomerData.NUM_SPRITES; i++)
        {
            GameObject newSprite = new GameObject("Customer Sprite " + i);
            SpriteRenderer renderer = newSprite.AddComponent<SpriteRenderer>();
            renderer.sprite = customerData.sprites[i];
            newSprite.transform.position = customerData.spriteOffsets[i];
            newSprite.transform.SetParent(customerObject.transform);
        }
    }
}