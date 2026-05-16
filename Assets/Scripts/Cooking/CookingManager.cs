public enum GamePhase
{
    Setup,
    Cooking
}

public class CookingManager : Singleton<CookingManager>
{
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Setup;

    public void StartDay()
    {
        CurrentPhase = GamePhase.Cooking;
        FoodGrab.CanMoveFood = true;
        ObjectGrab.CanMoveAppliances = false;
        StoryManager.Instance.InitRun();
        StoryManager.Instance.BeginDay();
        NextCustomer();
    }

    public void ServeOrder()
    {
        
    }

    public void NextCustomer()
    {
        // CustomerLineManager.Instance.Advance();
    }
}
