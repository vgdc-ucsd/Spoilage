using TMPro;
using UnityEngine;

public class ShopItemScript : MonoBehaviour
{
    public Texture2D hoverCursor;

    public GameState gameState;

    public ShopItem item;

    public TextMeshPro priceField;
    public TextMeshPro nameField;
    public TextMeshPro typeField;

    public SpriteRenderer imageField;


    private bool _bought = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateGUI();
    }

    bool canBuy()
    {
        return !_bought && gameState.Money >= item.price;
    }

    void OnMouseDown()
    {
        if (canBuy())
        {
            gameState.Money -= item.price;
            _bought = true;
            GetComponent<SpriteRenderer>().color *= Color.gray;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    void OnMouseEnter()
    {
        if (canBuy())
            Cursor.SetCursor(hoverCursor, new Vector2(hoverCursor.width, hoverCursor.height) / 2, CursorMode.Auto);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void UpdateGUI()
    {
        priceField.text = "$" + item.price;
        nameField.text = item.name;
        typeField.text = "- " + item.itemType + " -";
        imageField.sprite = item.icon;
        GetComponent<SpriteRenderer>().color = item.color;
        if (_bought)
            GetComponent<SpriteRenderer>().color *= Color.gray;
    }
}
