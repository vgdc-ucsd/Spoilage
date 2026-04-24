using UnityEngine;

public class LockLayout : MonoBehaviour
{
    // Static so InteractManager can see it without a reference
    public static bool IsLocked = false;

    [SerializeField] private GameObject _lockButtonObject; // Drag "Start Day" HERE

    private void Awake()
    {
        // Reset everything when the game starts
        IsLocked = false;
    }

    public void StartDay()
    {
        // 1. Flip the switch to TRUE
        IsLocked = true;

        // 2. Hide the button so it can't be clicked again
        _lockButtonObject.SetActive(false);

        Debug.Log("Day Started: Layout is now PERMANENTLY locked.");
    }
}
