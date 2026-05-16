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
        if (SetupManager.Instance.CurrentPhase == GamePhase.Setup)
        {
            SetupManager.Instance.StartCooking();
        }
        else
        {
            
        }
    }
}
