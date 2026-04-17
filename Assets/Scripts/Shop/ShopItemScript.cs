using UnityEngine;
using UnityEngine.InputSystem;

public class ShopItemScript : MonoBehaviour
{
    public Texture2D hoverCursor;

    public GameState gameState;

    public int price = 10;

    private bool _bought = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
}
