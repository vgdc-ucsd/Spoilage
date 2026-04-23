using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CustomerManager : Singleton<CustomerManager>
{
    private const string EYES_PATH = "Assets/Resources/Customers/Eyes/";
    private const string MOUTH_PATH = "Assets/Resources/Customers/NoseMouths/";
    private const string SPOILAGE_PATH = "Assets/Resources/Customers/Spoilage/";
    private const string FRONT_FOLDER = "Front/";
    private const string BACK_FOLDER = "Back/";
    private const string BASES_PATH = "Assets/Resources/Customers/Bases/";
    private const string CLOTHES_FOLDER = "/Clothes/";
    private const string HAIR_FOLDER = "/Hair/";

    private const string REGEX_NOT_META = "^(?!.*\\.meta)(?!.*reference).*";
    private const string REGEX_STATIC = "static.*$";
    private const string REGEX_ANGER = "anger.*$";
    private const string REGEX_DISGUST = "disgust.*$";
    private const string REGEX_TALKING = "talking.*$";
    private const string REGEX_WIDENING = "widening.*$";
    private const string REGEX_BLINKING = "blinking.*$";
    private const string REGEX_HAIR_FRONT = "hairTop.*$";
    private const string REGEX_HAIR_BACK = "hairBottom.*$";
    private const string REGEX_HAIR_SHADOW = "hairShadow.*$";
    
    private static Dictionary<string, Vector3> s_faceOffsets;
    //private static Regex;

    [SerializeField]
    private CustomerData[] _debug;


    void Start()
    {
        s_faceOffsets = new Dictionary<string, Vector3>
        {
            { "test", new Vector3(0, 0, 1.5f) }
        };


        _debug = new CustomerData[10];
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("Debug " + i);
            _debug[i] = GenerateCustomerData();
        } 
    }

    

    public static CustomerData GenerateCustomerData()
    {
        CustomerData newData = ScriptableObject.CreateInstance<CustomerData>();

        newData.spoilage = (CustomerData.Spoilage)UnityEngine.Random.Range(0, 2);
        newData.sprites = new Sprite[CustomerData.NUM_SPRITES];
        newData.patience = UnityEngine.Random.Range(0f, 1f); // TODO, talk to design

        string[] paths = new string[CustomerData.NUM_SPRITES];

        string eyesDir = getRandomElement(Directory.GetDirectories(EYES_PATH));
        // Using getRandomElement() mostly to make sure the list isn't empty.
        // If there are for some reason multiple files with the pattern 
        // something is wrong with the file structure.
        paths[(int)CustomerData.Indexes.EYES_OPEN] = getRandomElement(
            Directory.GetFiles(eyesDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META + REGEX_STATIC)).ToArray());
        paths[(int)CustomerData.Indexes.EYES_CLOSED] = getRandomElement(
            Directory.GetFiles(eyesDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META + REGEX_BLINKING)).ToArray());
        paths[(int)CustomerData.Indexes.EYES_ANGER] = getRandomElement(
            Directory.GetFiles(eyesDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META + REGEX_ANGER)).ToArray());
        paths[(int)CustomerData.Indexes.EYES_DISGUST] = getRandomElement(
            Directory.GetFiles(eyesDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META + REGEX_DISGUST)).ToArray());
        paths[(int)CustomerData.Indexes.EYES_WIDENING] = getRandomElement(
            Directory.GetFiles(eyesDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META + REGEX_WIDENING)).ToArray());

        string mouthDir = getRandomElement(Directory.GetDirectories(MOUTH_PATH));

        paths[(int)CustomerData.Indexes.MOUTH_OPEN] = getRandomElement(
            Directory.GetFiles(mouthDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META + REGEX_TALKING)).ToArray());
        paths[(int)CustomerData.Indexes.MOUTH_CLOSED] = getRandomElement(
            Directory.GetFiles(mouthDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META + REGEX_STATIC)).ToArray());
        paths[(int)CustomerData.Indexes.MOUTH_ANGER] = getRandomElement(
            Directory.GetFiles(mouthDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META + REGEX_ANGER)).ToArray());
        paths[(int)CustomerData.Indexes.MOUTH_DISGUST] = getRandomElement(
            Directory.GetFiles(mouthDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META + REGEX_DISGUST)).ToArray());
        

        paths[(int)CustomerData.Indexes.SPOILAGE_FRONT] = getRandomElement(
            Directory.GetFiles(SPOILAGE_PATH + FRONT_FOLDER).Where(path => Regex.IsMatch(path, REGEX_NOT_META)).ToArray());
        paths[(int)CustomerData.Indexes.SPOILAGE_BACK] = getRandomElement(
            Directory.GetFiles(SPOILAGE_PATH + BACK_FOLDER).Where(path => Regex.IsMatch(path, REGEX_NOT_META)).ToArray());

        string bodyDir = getRandomElement(Directory.GetDirectories(BASES_PATH));
        paths[(int)CustomerData.Indexes.BODY] = getRandomElement(
            Directory.GetFiles(bodyDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META)).ToArray());

        paths[(int)CustomerData.Indexes.CLOTHES] = getRandomElement(
            Directory.GetFiles(bodyDir + CLOTHES_FOLDER).Where(path => Regex.IsMatch(path, REGEX_NOT_META)).ToArray());

        string hairDir = getRandomElement(Directory.GetDirectories(bodyDir + HAIR_FOLDER));
        paths[(int)CustomerData.Indexes.HAIR_FRONT] = getRandomElement(
            Directory.GetFiles(hairDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META + REGEX_HAIR_FRONT)).ToArray());
        paths[(int)CustomerData.Indexes.HAIR_BACK] = getRandomElement(
            Directory.GetFiles(hairDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META + REGEX_HAIR_BACK)).ToArray());
        paths[(int)CustomerData.Indexes.HAIR_SHADOW] = getRandomElement(
            Directory.GetFiles(hairDir).Where(path => Regex.IsMatch(path, REGEX_NOT_META + REGEX_HAIR_SHADOW)).ToArray());


        for (int i = 0; i < CustomerData.NUM_SPRITES; i++)
        {
            if (paths[i] == null)
            {
                newData.sprites[i] = null;
                continue;
            } 

            if (i == (int) CustomerData.Indexes.EYES_CLOSED ||
                i == (int) CustomerData.Indexes.EYES_ANGER ||
                i == (int) CustomerData.Indexes.EYES_DISGUST ||
                i == (int) CustomerData.Indexes.EYES_WIDENING ||
                i == (int) CustomerData.Indexes.MOUTH_OPEN ||
                i == (int) CustomerData.Indexes.MOUTH_ANGER ||
                i == (int) CustomerData.Indexes.MOUTH_DISGUST)
            {
                // Get the second sprite of the sprite sheet
                Sprite[] sheet = Resources.LoadAll<Sprite>(trimPath(paths[i]));
                // Debug.Log("SPRITESHEET:");
                // for (int j = 0; j < sheet.Length; j++)
                // {
                //     Debug.Log(j + ": " + sheet[j]);
                // }
                if (sheet != null && sheet.Length >= 2)
                {
                    newData.sprites[i] = sheet[1];
                    Debug.Log(newData.sprites[i]);
                }
                //newData.sprites[i] = sheet != null && sheet.Length >= 2 ? sheet[1] : null;
            }
            
            newData.sprites[i] = Resources.Load<Sprite>(trimPath(paths[i]));
        }
        //s_faceOffsets.TryGetValue("test", out newData.faceOffset);
        try
        {
            //newData.faceOffset = s_faceOffsets["test"];
            newData.faceOffset = s_faceOffsets[bodyDir];
        } catch
        {
            Debug.LogWarning("Face offset not found for body " + bodyDir);
            newData.faceOffset = Vector3.zero;
        }

        return newData;
    }

    private static string getRandomElement(string[] arr)
    {
        if (arr == null || arr.Length <= 0) 
            return null;
        
        return arr[UnityEngine.Random.Range(0, arr.Length)];
    }

    private static string trimPath(string str)
    {
        if (str == null) { return null; }
        int startIndex = "Assets/Resources/".Length;
        int endOffset = ".png".Length;

        return str.Substring(startIndex, str.Length - endOffset - startIndex);
    }

    // TODO: Store a list of local offsets for the eyes and nose/mouth here
    // TODO: Store a list of global offsets for the eyes and nose/mouth here
}