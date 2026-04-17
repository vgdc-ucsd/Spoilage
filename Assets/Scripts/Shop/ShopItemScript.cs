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
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Debug.Log("Clicked!");
            }
        }
    }

    void OnMouseDown()
    {
        gameState.Money -= 10;
        Debug.Log("Clicked!!!");
    }
}
