using UnityEngine;

public class TrashCan : MonoBehaviour
{
    public void TrashItem(FoodGrab foodItem)
    {
        Destroy(foodItem.gameObject);
        Debug.Log("Food item trashed!");
    }
}
