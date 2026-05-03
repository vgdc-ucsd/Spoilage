using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayObject : MonoBehaviour
{
    public Transform canvas;
    public GameObject ingredientImagePrefab;
    public int lineLength;
    private int _numDots;
    private int _numIngredients;
    public GameObject orderDisplayPrefab;
    private GameObject _orderObject;
    private Item _order;
    private int _orderCostLength;
    private int _orderNameLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    _orderObject = GameObject.FindGameObjectWithTag("Order");
    _order = _orderObject.GetComponent<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateReceipt()
    {
        Debug.Log("HI");
        if (GameObject.FindGameObjectWithTag("Order Display Object") != null)
        {
            Debug.Log("Order Display Object already exists");
            return;
        }

        _numIngredients = _order.ingredients.Count;
        _orderCostLength = _order.itemCost.ToString().Length;
        _orderNameLength = _order.itemName.Length;
        _numDots = lineLength - _orderCostLength - _orderNameLength;
        List<Texture2D> orderIngredients = _order.ingredients;
        List<Texture2D> orderPreparations = _order.preparations;

        GameObject obj = Instantiate(orderDisplayPrefab, canvas);
        
        UnityEngine.UI.Image foodPicture = obj.transform.Find("Food Picture").GetComponent<UnityEngine.UI.Image>();
        TextMeshProUGUI itemName = obj.transform.Find("Item Name").GetComponent<TextMeshProUGUI>();
        Transform ingredients = obj.transform.Find("Ingredients");

        Sprite foodPictureSprite = Sprite.Create(_order.foodPicture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f), 100.0f);
        foodPicture.sprite = foodPictureSprite;
        foodPicture.color = Color.white;
        
        itemName.text = _order.itemName + new string('.', _numDots) + _order.itemCost.ToString();
        
        int overallHeight = 0;

        for (int i = 0; i < _numIngredients; i++)
        {
            GameObject ingredientContainer = Instantiate(ingredientImagePrefab, ingredients);

            UnityEngine.UI.Image ingredientImage = ingredientContainer.transform.Find("Ingredient").GetComponent<UnityEngine.UI.Image>();
            UnityEngine.UI.Image preparationImage = ingredientContainer.transform.Find("Preparation").GetComponent<UnityEngine.UI.Image>();

            Sprite ingredientSprite = Sprite.Create(_order.ingredients[i], new Rect(0, 0, _order.ingredients[i].width, _order.ingredients[i].height), new Vector2(0.5f, 0.5f), 100.0f);
            Sprite preparationSprite = Sprite.Create(_order.preparations[i], new Rect(0, 0, _order.preparations[i].width, _order.preparations[i].height), new Vector2(0.5f, 0.5f), 100.0f);
            ingredientImage.sprite = ingredientSprite;
            preparationImage.sprite = preparationSprite;

            overallHeight += _order.ingredients[i].height;
        }

        RectTransform rt = ingredients.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, overallHeight + (_numIngredients - 1) * 5);

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
