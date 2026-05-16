using UnityEngine;

public class SetupManager : Singleton<SetupManager>
{
    public override void Awake()
    {
        base.Awake();
        FoodGrab.CanMoveFood = false;
        ObjectGrab.CanMoveAppliances = true;
    }

    public void StartCooking()
    {
        FoodGrab.CanMoveFood = true;
        ObjectGrab.CanMoveAppliances = false;
        Debug.Log("Phase: Cooking");
    }

    public void StartSetup()
    {
        FoodGrab.CanMoveFood = false;
        ObjectGrab.CanMoveAppliances = true;
        Debug.Log("Phase: Setup");
    }
}