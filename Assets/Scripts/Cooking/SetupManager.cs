using UnityEngine;
public enum GamePhase
{
    Setup,
    Cooking
}

public class SetupManager : MonoBehaviour
{
    public static SetupManager Instance { get; private set; }
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Setup;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        FoodGrab.CanMoveFood = false;
        ObjectGrab.CanMoveAppliances = true;
    }

    public void StartCooking()
    {
        CurrentPhase = GamePhase.Cooking;
        FoodGrab.CanMoveFood = true;
        ObjectGrab.CanMoveAppliances = false;
        Debug.Log("Phase: Cooking");
    }

    public void StartSetup()
    {
        CurrentPhase = GamePhase.Setup;
        FoodGrab.CanMoveFood = false;
        ObjectGrab.CanMoveAppliances = true;
        Debug.Log("Phase: Setup");
    }
}