using System.Collections.Generic;
using UnityEngine;
using System;   

[System.Serializable]
public class Customer
{
    public CustomerData customerData;
    public int spoilage;

}

public class CustomerData
{
    public List<GameObject> sprites;
    public List<string> dialogues;
    

    // Task for Sprint 1
    private void GenerateSprite()
    {
        // Choose a random sprite from the asset folder
        // Determine sprite position and scale
    }

    private void GetDialogue()
    {
        throw new NotImplementedException();
    }
}