using UnityEngine;

public class LockLayout : MonoBehaviour
{
    public static bool IsLocked = false;

    [SerializeField] private GameObject _lockButtonObject;

    private void Awake()
    {
        // Reset everything when the game starts
        IsLocked = false;
    }

    public void StartDay()
    {
        // 1. Flip the switches to TRUE
        IsLocked = true;
        FoodGrab.CanMoveFood = true;

        // 2. Hide the button so it can't be clicked again
        _lockButtonObject.SetActive(false);

        Debug.Log("Day Started: Layout is now PERMANENTLY locked.");
    }
}
