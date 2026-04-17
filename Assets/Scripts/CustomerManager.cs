using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    private const string EYES_PATH = "Customers/Eyes/";
    private const string NOSE_MOUTH_PATH = "Customers/NoseMouths/";
    private const string SPOILAGE_PATH = "Customers/Spoilage/";
    private const string FRONT_FOLDER = "Front/";
    private const string BACK_FOLDER = "Back/";
    private const string BASES_PATH = "Customers/Bases/";
    private const string CLOTHES_FOLDER = "Clothes/";
    private const string HAIR_FOLDER = "Hair/";

    private const int HAIR_FRONT = 0;
    private const int HAIR_BACK = 1;
    private const int HAIR_SHADOW = 2;

    
    public static CustomerData generateCustomerData()
    {
        CustomerData newData = new CustomerData
        {
            spoilage = (CustomerData.Spoilage) UnityEngine.Random.Range(0, 2),
            sprites = new Sprite[CustomerData.NUM_SPRITES],
            patience = UnityEngine.Random.Range(0f, 1f) // TODO, talk to design
        };

        string eyesPath = getRandomElement(Directory.GetFiles(EYES_PATH));
        string noseMouthPath = getRandomElement(Directory.GetFiles(NOSE_MOUTH_PATH));
        string spoilageFrontPath = getRandomElement(Directory.GetFiles(SPOILAGE_PATH + FRONT_FOLDER));
        string spoilageBackPath = getRandomElement(Directory.GetFiles(SPOILAGE_PATH + BACK_FOLDER));

        string bodyDir = getRandomElement(Directory.GetDirectories(BASES_PATH));
        string bodyPath = getRandomElement(Directory.GetFiles(bodyDir));

        string clothesPath = getRandomElement(Directory.GetFiles(bodyDir + CLOTHES_FOLDER));

        string hairDir = getRandomElement(Directory.GetDirectories(bodyDir + HAIR_FOLDER));        
        string[] hairPaths = Directory.GetFiles(hairDir);
        Array.Sort(hairPaths);


        newData.sprites[(int)CustomerData.Indexes.BODY] = Resources.Load<Sprite>(bodyPath);
        newData.sprites[(int)CustomerData.Indexes.HAIR_FRONT] = Resources.Load<Sprite>(hairPaths[HAIR_FRONT]);

        newData.sprites[(int)CustomerData.Indexes.HAIR_BACK] = Resources.Load<Sprite>(hairPaths[HAIR_BACK]);
        newData.sprites[(int)CustomerData.Indexes.HAIR_SHADOW] = Resources.Load<Sprite>(hairPaths[HAIR_SHADOW]);
        newData.sprites[(int)CustomerData.Indexes.CLOTHES] = Resources.Load<Sprite>(clothesPath);
        newData.sprites[(int)CustomerData.Indexes.NOSE_MOUTH] = Resources.Load<Sprite>(noseMouthPath);
        newData.sprites[(int)CustomerData.Indexes.EYES] = Resources.Load<Sprite>(eyesPath);
        newData.sprites[(int)CustomerData.Indexes.SPOILAGE_FRONT] = Resources.Load<Sprite>(spoilageFrontPath);
        newData.sprites[(int)CustomerData.Indexes.SPOILAGE_BACK] = Resources.Load<Sprite>(spoilageBackPath);
        




        return newData;
    }

    private static string getRandomElement(string[] arr)
    {
        return arr[UnityEngine.Random.Range(0, arr.Length - 1)];
    }
}