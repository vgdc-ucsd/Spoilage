using UnityEngine;
using UnityEngine.InputSystem;

public class ShopItemScript : MonoBehaviour
{
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

    void OnMouseDown()
    {
        if (!_bought && gameState.Money >= price)
        {
            gameState.Money -= price;
            _bought = true;
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }
}
