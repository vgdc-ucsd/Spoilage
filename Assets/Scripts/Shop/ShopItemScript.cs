using UnityEngine;
using UnityEngine.InputSystem;

public class ShopItemScript : MonoBehaviour
{
    public GameState gameState;

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
        gameState.Money -= 10;
    }
}
