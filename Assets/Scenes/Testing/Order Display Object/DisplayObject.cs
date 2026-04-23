using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayObject : MonoBehaviour
{
    public Transform canvas;
    public GameObject ingredientImagePrefab;
    public int lineLength;
    private int _numAsterisks;
    private int _numIngredients;
    public GameObject orderDisplayPrefab;
    public GameObject orderObject;
    public Item order;
    private int _orderCostLength;
    private int _orderNameLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateReceipt(Item item)
    {
        if (GameObject.FindGameObjectWithTag("Order Display Object") != null)
        {
            Debug.Log("Order Display Object already exists");
            return;
        }

        orderObject = GameObject.FindGameObjectWithTag("Order");
        order = orderObject.GetComponent<Item>();

        _numIngredients = order.ingredients.Count;
        _orderCostLength = order.itemCost.ToString().Length;
        _orderNameLength = order.itemName.Length;
        _numAsterisks = lineLength - _orderCostLength - _orderNameLength;

        GameObject obj = Instantiate(orderDisplayPrefab, canvas);
        
        UnityEngine.UI.Image foodPicture = obj.transform.Find("Food Picture").GetComponent<UnityEngine.UI.Image>();
        TextMeshProUGUI itemName = obj.transform.Find("Item Name").GetComponent<TextMeshProUGUI>();
        Transform ingredients = obj.transform.Find("Ingredients");
        UnityEngine.UI.Image barCode = obj.transform.Find("Barcode").GetComponent<UnityEngine.UI.Image>();
        TextMeshProUGUI barCodeNumber = obj.transform.Find("Barcode Number").GetComponent<TextMeshProUGUI>();
        UnityEngine.UI.Image signature = obj.transform.Find("Signature").GetComponent<UnityEngine.UI.Image>();

        Sprite foodPictureSprite = Sprite.Create(order.foodPicture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f), 100.0f);
        foodPicture.sprite = foodPictureSprite;
        foodPicture.color = Color.white;
        
        itemName.text = "<align=\"flush\">" + order.itemName + new string('*', _numAsterisks) + order.itemCost.ToString();
        
        RectTransform rt = ingredients.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, _numIngredients * 100 + (_numIngredients - 1) * 5);

        foreach (Texture2D ingredient in order.ingredients)
        {
            GameObject ingredientImageObject = Instantiate(ingredientImagePrefab, ingredients);

            UnityEngine.UI.Image ingredientImage = ingredientImageObject.GetComponent<UnityEngine.UI.Image>();

            Sprite ingredientSprite = Sprite.Create(ingredient, new Rect(0, 0, 400, 100), new Vector2(0.5f, 0.5f), 100.0f);
            ingredientImage.sprite = ingredientSprite;
        }

        Sprite barCodeSprite = Sprite.Create(order.barCode, new Rect(0, 0, 400, 100), new Vector2(0.5f, 0.5f), 100.0f);
        barCode.sprite = barCodeSprite;
        barCode.color = Color.white;
        
        barCodeNumber.text = order.barCodeNumber;

        Sprite signatureSprite = Sprite.Create(order.signature, new Rect(0, 0, 400, 100), new Vector2(0.5f, 0.5f), 100.0f); 
        signature.sprite = signatureSprite;
        signature.color = Color.white;
    }

    public void DeleteReceipt()
    {
        GameObject receipt = GameObject.FindGameObjectWithTag("Order Display Object");

        if (receipt)
        {
            Destroy(receipt);
        }
        else
        {
            Debug.Log("Order Display Object does not exist");
        }
    }
}
