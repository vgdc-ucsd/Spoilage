using UnityEngine;
using UnityEngine.UI;

public class MoneyScript : MonoBehaviour
{
    public GameState gameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState.moneyText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
