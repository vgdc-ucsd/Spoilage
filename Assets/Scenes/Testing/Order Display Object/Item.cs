using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Texture2D barCode;
    public String barCodeNumber;
    public List<Texture2D> ingredients;
    public String itemName;
    public Texture2D foodPicture;
    public Texture2D signature;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        barCodeNumber = generateBarCodeNumber();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private String generateBarCodeNumber()
    {
        System.Random rnd = new System.Random();
        String output = "";

        for (int i=0; i<9; i++)
        {
            output += rnd.Next(0, 10).ToString();
        }

        return output;
    }
}
