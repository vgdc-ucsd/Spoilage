using UnityEngine;

public class CallBell : MonoBehaviour
{
    void Start()
    {
        LockLayout.IsLocked = false;
        FoodGrab.CanMoveFood = false;
    }

    public void RingBell()
    {
        if (CookingManager.Instance.CurrentPhase == GamePhase.Setup)
        {
            CookingManager.Instance.StartDay();
        }
        else
        {
            CookingManager.Instance.ServeOrder();
        }
    }
}
