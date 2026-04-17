using TMPro;
using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    public GameState gameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState.moneyText = gameObject.GetComponent<TextMeshProUGUI>();
        Debug.Log(gameState.moneyText);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
