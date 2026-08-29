using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CustomerManager : Singleton<CustomerManager>
{
    public GameObject CustomerPrefab;
    public CustomerData[] PresetCustomerData;
    [SerializeField]
    private RectTransform _customerTransform;

    private const string EYES_PATH = "Art/Customers/Eyes";
    private const string MOUTH_PATH = "Art/Customers/NoseMouths";
    private const string BASES_PATH = "Art/Customers/Bases";
    private const string CLOTHES_FOLDER = "/Clothes";
    private const string HAIR_FOLDER = "/Hair";

    private const string SPOILAGE_II_PATH = "Art/Customers/Spoilage/Fully Spoiled";

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
    private const string REGEX_SPOILAGE_BASE = "body.*$";
    private const string REGEX_SPOILAGE_EYES = "eyes.*$";
    private const string REGEX_SPOILAGE_TENDRILS = "tendrils.*$";

    private static readonly string[] s_eyesDirs =
    {
        "eyes_eyebrows100", "eyes_eyebrows101", "eyes_eyebrows102", "eyes_eyebrows103", "eyes_eyebrows104",
        "eyes_eyebrows167", "eyes_eyebrows248", "eyes_eyebrows255", "eyes_eyebrows257", "eyes_eyebrows258",
        "eyes_eyebrows259", "eyes_eyebrows260", "eyes_eyebrows261", "eyes_eyebrows277", "eyes_eyebrows283",
        "eyes_eyebrows286", "eyes_eyebrows288", "eyes_eyebrows292", "eyes_eyebrows332"
    };

    private static readonly string[] s_mouthDirs =
    {
        "nose_mouth100", "nose_mouth101", "nose_mouth102", "nose_mouth103", "nose_mouth104",
        "nose_mouth167", "nose_mouth248", "nose_mouth255", "nose_mouth257", "nose_mouth258",
        "nose_mouth259", "nose_mouth260", "nose_mouth261", "nose_mouth277", "nose_mouth283",
        "nose_mouth286", "nose_mouth288", "nose_mouth292", "nose_mouth332"
    };

    private static readonly string[] s_baseDirs =
    {
        "Character Base #167", "Character Base #255", "Character Base #256", "Character Base #257",
        "Character Base #258", "Character Base #259", "Character Base #260", "Character Base #261",
        "Character Base #277", "Character Base #284", "Character Base #286", "Character Base #287",
        "Character Base #292", "Character Base #332"
    };

    private static readonly Dictionary<string, Vector3> s_faceOffsets = new()
    {
        { "Art/Customers/Bases/Character Base #167", new Vector3(-12f, 10f, 0)},
        { "Art/Customers/Bases/Character Base #255", new Vector3(-2f, 116f, 0)},
        { "Art/Customers/Bases/Character Base #256", new Vector3(1f, -7f, 0)},
        { "Art/Customers/Bases/Character Base #257", new Vector3(-1f, -74f, 0)},
        { "Art/Customers/Bases/Character Base #258", new Vector3(9f, -66f, 0)},
        { "Art/Customers/Bases/Character Base #259", new Vector3(-8f, 104f, 0)},
        { "Art/Customers/Bases/Character Base #260", new Vector3(4f, 32f, 0)},
        { "Art/Customers/Bases/Character Base #261", new Vector3(-11f, 81f, 0)},
        { "Art/Customers/Bases/Character Base #277", new Vector3(3f, 125f, 0)},
        { "Art/Customers/Bases/Character Base #284", new Vector3(6f, -90f, 0)},
        { "Art/Customers/Bases/Character Base #286", new Vector3(0f, 0f, 0)},
        { "Art/Customers/Bases/Character Base #287", new Vector3(10f, -24f, 0)},
        { "Art/Customers/Bases/Character Base #292", new Vector3(5f, 55f, 0)},
        { "Art/Customers/Bases/Character Base #307", new Vector3(-25f, -65f, 0)},
        { "Art/Customers/Bases/Character Base #332", new Vector3(16f, -11f, 0)},
    };

    public Customer GenerateCustomer()
    {
        return GenerateCustomer(GenerateCustomerData(), null); // TODO
    }

    public Customer GenerateCustomer(CustomerData customerData, CustomerDialogue dialogue)
    {
        if (customerData == null)
        {
            customerData = GenerateCustomerData();
        }

        Customer instantiatedCustomer = Instantiate(CustomerPrefab, _customerTransform).GetComponent<Customer>();
        instantiatedCustomer.customerData = customerData;
        instantiatedCustomer.customerObject = instantiatedCustomer.gameObject;
        instantiatedCustomer.SetDialogue(dialogue);

        instantiatedCustomer.InitializeCustomer();

        return instantiatedCustomer;
    }

    public CustomerData GenerateCustomerData()
    {
        CustomerData newData = ScriptableObject.CreateInstance<CustomerData>();

        // Curves? Static distribution? I made the current numbers up randomly
        int day = 0;
        float spoilageSeed = UnityEngine.Random.Range(0f, 1f);
        float stage1Threshold;
        float stage2Threshold;

        if (SaveManager.Instance?.Player != null)
        {
            day = SaveManager.Instance.Player.Day;
        }

        if (day < 2) 
        {
            stage1Threshold = 1.0f;
            stage2Threshold = 1.0f;
        }
        else if (day < 6) // Stage 1 unlocked
        {
            stage1Threshold = 0.7f;
            stage2Threshold = 1.0f;
        }
        else // Stage 2 unlocked
        {
            stage1Threshold = 0.7f;
            stage2Threshold = 0.9f;
        }

        if (spoilageSeed <= stage1Threshold)
        {
            newData.spoilage = CustomerData.Spoilage.UNSPOILED;
        }
        else if (spoilageSeed <= stage2Threshold)
        {
            newData.spoilage = CustomerData.Spoilage.STAGE_I;
        }
        else // 10% chance
        {
            newData.spoilage = CustomerData.Spoilage.STAGE_II;
        }

        //newData.spoilage = (CustomerData.Spoilage)UnityEngine.Random.Range(0, 2);

        newData.sprites = new Sprite[CustomerData.NUM_SPRITES];
        newData.patience = UnityEngine.Random.Range(0f, 1f); // TODO, talk to design


        // Spoilage-dependent choices (sprites and symptom)
        switch (newData.spoilage)
        {
            case CustomerData.Spoilage.STAGE_II:
                AssignStageIISprites(newData);
                break;

            case CustomerData.Spoilage.STAGE_I:
                newData.spoilageSymptom = GenerateSymptom();
                goto case CustomerData.Spoilage.UNSPOILED;

            case CustomerData.Spoilage.UNSPOILED:
                AssignNormalSprites(newData);
                break;
        }

        // Customer order
        CustomerOrderDatabase customerOrderDatabase = CustomerOrderDatabase.Instance;

        int orderCount = customerOrderDatabase.PickDishCount(0.5f); // TODO: Get actual game progress.

        for (int i = 0; i < orderCount; i++)
        {
            customerOrderDatabase.GenerateCustomerOrder(newData);
        }

        return newData;
    }

    private void AssignNormalSprites(CustomerData newData)
    {
        string eyesDirPath = EYES_PATH + "/" + getRandomElement(s_eyesDirs);
        Sprite[] eyeSprites = Resources.LoadAll<Sprite>(eyesDirPath);
        newData.sprites[(int)CustomerData.Indexes.EYES_OPEN]     = getRandomSprite(eyeSprites, REGEX_NOT_META + REGEX_STATIC);
        newData.sprites[(int)CustomerData.Indexes.EYES_CLOSED]   = getSheetFrame1(eyeSprites, REGEX_NOT_META + REGEX_BLINKING);
        newData.sprites[(int)CustomerData.Indexes.EYES_ANGER]    = getSheetFrame1(eyeSprites, REGEX_NOT_META + REGEX_ANGER);
        newData.sprites[(int)CustomerData.Indexes.EYES_DISGUST]  = getSheetFrame1(eyeSprites, REGEX_NOT_META + REGEX_DISGUST);
        newData.sprites[(int)CustomerData.Indexes.EYES_WIDENING] = getSheetFrame1(eyeSprites, REGEX_NOT_META + REGEX_WIDENING);

        string mouthDirPath = MOUTH_PATH + "/" + getRandomElement(s_mouthDirs);
        Sprite[] mouthSprites = Resources.LoadAll<Sprite>(mouthDirPath);
        newData.sprites[(int)CustomerData.Indexes.MOUTH_OPEN]    = getSheetFrame1(mouthSprites, REGEX_NOT_META + REGEX_TALKING);
        newData.sprites[(int)CustomerData.Indexes.MOUTH_CLOSED]  = getRandomSprite(mouthSprites, REGEX_NOT_META + REGEX_STATIC);
        newData.sprites[(int)CustomerData.Indexes.MOUTH_ANGER]   = getSheetFrame1(mouthSprites, REGEX_NOT_META + REGEX_ANGER);
        newData.sprites[(int)CustomerData.Indexes.MOUTH_DISGUST] = getSheetFrame1(mouthSprites, REGEX_NOT_META + REGEX_DISGUST);

        string baseDirPath = BASES_PATH + "/" + getRandomElement(s_baseDirs);

        // Body: load recursively but filter to only body sprites (excludes clothes, hair, reference images)
        Sprite[] allBaseSprites = Resources.LoadAll<Sprite>(baseDirPath);
        newData.sprites[(int)CustomerData.Indexes.BODY] = getRandomSprite(allBaseSprites, REGEX_NOT_META + REGEX_SPOILAGE_BASE);

        Sprite[] clothesSprites = Resources.LoadAll<Sprite>(baseDirPath + CLOTHES_FOLDER);
        newData.sprites[(int)CustomerData.Indexes.CLOTHES] = getRandomSprite(clothesSprites, REGEX_NOT_META);

        // Hair: pick a random style group (A/B/C/D) then load the matching set
        Sprite[] allHairSprites = Resources.LoadAll<Sprite>(baseDirPath + HAIR_FOLDER);
        HashSet<string> hairGroupSet = new();
        foreach (Sprite s in allHairSprites)
        {
            Match m = Regex.Match(s.name, @"hair(?:Top|Bottom|Shadow)_([A-Z])");
            if (m.Success) hairGroupSet.Add(m.Groups[1].Value);
        }
        string selectedHairGroup = getRandomElement(hairGroupSet.ToArray());
        if (selectedHairGroup != null)
        {
            newData.sprites[(int)CustomerData.Indexes.HAIR_FRONT] = getRandomSprite(allHairSprites,
                $@"^(?!.*reference).*hairTop_{selectedHairGroup}($|_)");
            newData.sprites[(int)CustomerData.Indexes.HAIR_BACK] = getRandomSprite(allHairSprites,
                $@"^(?!.*reference).*hairBottom_{selectedHairGroup}($|_)");
            newData.sprites[(int)CustomerData.Indexes.HAIR_SHADOW] = getRandomSprite(allHairSprites,
                $@"^(?!.*reference).*hairShadow_{selectedHairGroup}($|_)");
        }

        if (s_faceOffsets.ContainsKey(baseDirPath))
        {
            newData.faceOffset = s_faceOffsets[baseDirPath];
        }
        else
        {
            Debug.LogWarning("Face offset not found for body " + baseDirPath);
            newData.faceOffset = new Vector3(0f, 175f, 0);
        }
    }

    private void AssignStageIISprites(CustomerData newData)
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>(SPOILAGE_II_PATH);

        newData.sprites[(int)CustomerData.Indexes.BODY] =
            getRandomSprite(allSprites, REGEX_NOT_META + REGEX_SPOILAGE_BASE);

        // Pick a random eye spritesheet (eyes1 or eyes2), then assign open/closed frames
        Sprite[] eyeSprites = allSprites
            .Where(s => Regex.IsMatch(s.name, REGEX_NOT_META + REGEX_SPOILAGE_EYES))
            .ToArray();
        string[] eyeSheetBaseNames = eyeSprites
            .Select(s => Regex.Replace(s.name, @"_[01]$", ""))
            .Distinct()
            .ToArray();
        string selectedEyeSheet = getRandomElement(eyeSheetBaseNames);
        if (selectedEyeSheet != null)
        {
            newData.sprites[(int)CustomerData.Indexes.EYES_OPEN] =
                eyeSprites.FirstOrDefault(s => s.name == selectedEyeSheet + "_0");
            newData.sprites[(int)CustomerData.Indexes.EYES_CLOSED] =
                eyeSprites.FirstOrDefault(s => s.name == selectedEyeSheet + "_1");
        }

        Sprite[] tendrilSprites = allSprites
            .Where(s => Regex.IsMatch(s.name, REGEX_NOT_META + REGEX_SPOILAGE_TENDRILS))
            .ToArray();
        newData.sprites[(int)CustomerData.Indexes.TENDRILS_1] =
            tendrilSprites.FirstOrDefault(s => s.name.EndsWith("_0"));
        newData.sprites[(int)CustomerData.Indexes.TENDRILS_2] =
            tendrilSprites.FirstOrDefault(s => s.name.EndsWith("_1"));

        // Customer order
        CustomerOrderDatabase customerOrderDatabase = CustomerOrderDatabase.Instance;
        customerOrderDatabase.GenerateCustomerOrder(newData);

        // int orderCount = customerOrderDatabase.PickDishCount(0.5f); // TODO: Get actual game progress.

        // for (int i = 0; i < orderCount; i++)
        // {
        //     Recipe order = customerOrderDatabase.GenerateCustomerOrder();

        //     if (order != null)
        //     {
        //         newData.orders.Add(order);
        //     }
        // }

        // return newData;
    }

    public static AbstractSpoilageSymptom GenerateSymptom()
    {
        Type randomSymptomType = AbstractSpoilageSymptom.symptomTypes[
            UnityEngine.Random.Range(0, AbstractSpoilageSymptom.symptomTypes.Length)
        ];
        AbstractSpoilageSymptom symptom = (AbstractSpoilageSymptom)
            ScriptableObject.CreateInstance(randomSymptomType);

        return symptom;
    }

    private static Sprite getRandomSprite(Sprite[] sprites, string pattern)
    {
        Sprite[] filtered = sprites.Where(s => Regex.IsMatch(s.name, pattern)).ToArray();
        if (filtered.Length == 0) return null;
        return filtered[UnityEngine.Random.Range(0, filtered.Length)];
    }

    // Returns the second frame (_1) of a spritesheet whose name matches pattern
    private static Sprite getSheetFrame1(Sprite[] sprites, string pattern)
    {
        Sprite[] filtered = sprites
            .Where(s => Regex.IsMatch(s.name, pattern) && s.name.EndsWith("_1"))
            .ToArray();
        if (filtered.Length == 0) return null;
        return filtered[UnityEngine.Random.Range(0, filtered.Length)];
    }

    private static string getRandomElement(string[] arr)
    {
        if (arr == null || arr.Length <= 0)
            return null;

        return arr[UnityEngine.Random.Range(0, arr.Length)];
    }

    /*TODO: CHOOSES DIALOGUE POOL BASED ON PLAYER MORALITY.
    public static int chooseDialoguePool(double moralityStat)
    {

    }
    */

    // TODO: Store a list of local offsets for the eyes and nose/mouth here
    // TODO: Store a list of global offsets for the eyes and nose/mouth here
}
