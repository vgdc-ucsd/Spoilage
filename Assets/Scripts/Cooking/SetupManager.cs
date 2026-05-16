using UnityEngine;
public enum GamePhase
{
    Setup,
    Cooking
}

public class SetupManager : Singleton<SetupManager>
{
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Setup;

    public override void Awake()
    {
        base.Awake();
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