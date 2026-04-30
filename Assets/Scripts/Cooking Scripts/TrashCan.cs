using UnityEngine;

public class TrashCan : MonoBehaviour
{
    public void Trash(FoodGrab food)
    {
        Debug.Log("Food thrown in trash can");
        Destroy(food.gameObject);
    }
}
