using UnityEngine;

public class StoveTops : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlaceFood(FoodGrab food)
    {
        food.transform.position = transform.position;
        Debug.Log("Food on Grill");
    }
}
