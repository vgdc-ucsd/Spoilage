using TMPro;
using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    public GameState gameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameState.moneyText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        gameState.Money = gameState.Money;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
