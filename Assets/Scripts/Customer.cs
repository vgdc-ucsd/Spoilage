using System.Collections.Generic;
using UnityEngine;
using System;

public class Customer
{
    public CustomerData customerData;
    public GameObject customerObject;

    public Customer(CustomerData data)
    {
        if (data != null)
        {
            customerData = data;
        }
        else
        {
            customerData = CustomerManager.generateCustomerData();
        }
    }

    public GameObject GenerateCustomerGameObject()
    {
        GameObject customer = new GameObject("Customer");
        for (int i = 0; i < customerData.sprites.Length; i++)
        {
            GameObject newSprite = new GameObject("Customer Sprite " + i);
            SpriteRenderer renderer = newSprite.AddComponent<SpriteRenderer>();
            renderer.sprite = customerData.sprites[i];
            newSprite.transform.SetParent(customer.transform);
            newSprite.transform.position += customerData.spriteOffsets[i];
        }
        return customer;
    }
}