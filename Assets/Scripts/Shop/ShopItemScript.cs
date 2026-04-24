using TMPro;
using UnityEngine;

public class ShopItemScript : MonoBehaviour
{
    public Texture2D hoverCursor;

    public GameState gameState;

    public int price = 10;

    private TextMeshPro _priceField;

    private bool _bought = false;

    void Awake()
    {
        if (_priceField == null)
        {
            _priceField = GetComponentInChildren<TextMeshPro>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdatePrice();
    }

    // Update is called once per frame
    void Update()
    {

    }

    bool canBuy()
    {
        return !_bought && gameState.Money >= price;
    }

    void OnMouseDown()
    {
        if (canBuy())
        {
            gameState.Money -= price;
            _bought = true;
            GetComponent<SpriteRenderer>().color = Color.gray;
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

    public void UpdatePrice()
    {
        _priceField.text = "$" + price;
    }
}
