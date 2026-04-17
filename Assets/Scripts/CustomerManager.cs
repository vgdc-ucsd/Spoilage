using System;
using System.IO;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    private const string EYES_PATH = "Assets/Resources/Customers/Eyes/";
    private const string NOSE_MOUTH_PATH = "Assets/Resources/Customers/NoseMouths/";
    private const string SPOILAGE_PATH = "Assets/Resources/Customers/Spoilage/";
    private const string FRONT_FOLDER = "Front/";
    private const string BACK_FOLDER = "Back/";
    private const string BASES_PATH = "Assets/Resources/Customers/Bases/";
    private const string CLOTHES_FOLDER = "/Clothes/";
    private const string HAIR_FOLDER = "/Hair/";

    private const string PNG = "*.png";

    private const int HAIR_FRONT = 0;
    private const int HAIR_BACK = 1;
    private const int HAIR_SHADOW = 2;


    [SerializeField]
    public CustomerData debug;

    void Start()
    {
        debug = GenerateCustomerData();
    }


    public static CustomerData GenerateCustomerData()
    {
        CustomerData newData = ScriptableObject.CreateInstance<CustomerData>();

        newData.spoilage = (CustomerData.Spoilage) UnityEngine.Random.Range(0, 2);
        newData.sprites = new Sprite[CustomerData.NUM_SPRITES];
        newData.patience = UnityEngine.Random.Range(0f, 1f); // TODO, talk to design

        Debug.Log("Choosing eye directory");
        string eyesDir = getRandomElement(Directory.GetDirectories(EYES_PATH));
        Debug.Log(eyesDir);
        string[] eyesPaths = Directory.GetFiles(eyesDir, PNG);
        Array.Sort(eyesPaths);

        
        string eyesOpenPath = eyesPaths[0];
        string eyesClosedPath = eyesPaths[1];

        Debug.Log("Choosing NoseMouth directory");
        string noseMouthDir = getRandomElement(Directory.GetDirectories(NOSE_MOUTH_PATH));
        string[] noseMouthPaths = Directory.GetFiles(noseMouthDir, PNG);
        Array.Sort(noseMouthPaths);

        string noseMouthOpenPath = noseMouthPaths[0];
        string noseMouthClosedPath = noseMouthPaths[1];

        Debug.Log("Choosing spoilage front");        
        string spoilageFrontPath = getRandomElement(Directory.GetFiles(SPOILAGE_PATH + FRONT_FOLDER, PNG));
        Debug.Log("Choosing spoilage back");        
        string spoilageBackPath = getRandomElement(Directory.GetFiles(SPOILAGE_PATH + BACK_FOLDER, PNG));

        Debug.Log("Choosing body dir");
        string bodyDir = getRandomElement(Directory.GetDirectories(BASES_PATH));
        Debug.Log("Choosing body sprite");
        string bodyPath = getRandomElement(Directory.GetFiles(bodyDir, PNG));

        Debug.Log("Choosing clothes sprite");
        string clothesPath = getRandomElement(Directory.GetFiles(bodyDir + CLOTHES_FOLDER, PNG));

        Debug.Log("Choosing hair dir");
        string hairDir = getRandomElement(Directory.GetDirectories(bodyDir + HAIR_FOLDER));        
        string[] hairPaths = Directory.GetFiles(hairDir, PNG);
        Array.Sort(hairPaths);
        Debug.Log(trimPath(bodyPath));

        newData.sprites[(int)CustomerData.Indexes.BODY] = Resources.Load<Sprite>(trimPath(bodyPath));
        //newData.sprites[(int)CustomerData.Indexes.BODY] = Resources.Load<Sprite>("Customers/Bases/BodyA/base287_body_C");
        newData.sprites[(int)CustomerData.Indexes.HAIR_FRONT] = Resources.Load<Sprite>(trimPath(hairPaths[HAIR_FRONT]));
        newData.sprites[(int)CustomerData.Indexes.HAIR_BACK] = Resources.Load<Sprite>(trimPath(hairPaths[HAIR_BACK]));
        newData.sprites[(int)CustomerData.Indexes.HAIR_SHADOW] = Resources.Load<Sprite>(trimPath(hairPaths[HAIR_SHADOW]));
        newData.sprites[(int)CustomerData.Indexes.CLOTHES] = Resources.Load<Sprite>(trimPath(clothesPath));
        newData.sprites[(int)CustomerData.Indexes.NOSE_MOUTH_OPEN] = Resources.Load<Sprite>(trimPath(noseMouthOpenPath));
        newData.sprites[(int)CustomerData.Indexes.NOSE_MOUTH_CLOSED] = Resources.Load<Sprite>(trimPath(noseMouthClosedPath));
        newData.sprites[(int)CustomerData.Indexes.EYES_OPEN] = Resources.Load<Sprite>(trimPath(eyesOpenPath));
        newData.sprites[(int)CustomerData.Indexes.EYES_CLOSED] = Resources.Load<Sprite>(trimPath(eyesClosedPath));
        newData.sprites[(int)CustomerData.Indexes.SPOILAGE_FRONT] = Resources.Load<Sprite>(trimPath(spoilageFrontPath));
        newData.sprites[(int)CustomerData.Indexes.SPOILAGE_BACK] = Resources.Load<Sprite>(trimPath(spoilageBackPath));
        
        return newData;
    }

    private static string getRandomElement(string[] arr)
    {
        return arr[UnityEngine.Random.Range(0, arr.Length)];
    }

    private static string trimPath(string str)
    {
        int startIndex = "Assets/Resources/".Length;
        int endOffset = ".png".Length;

        return str.Substring(startIndex, str.Length - endOffset - startIndex);
    }
}