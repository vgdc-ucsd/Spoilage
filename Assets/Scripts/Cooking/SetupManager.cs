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

    // Before this day is cozy music, after is horror!
    private const int COZY_MUSIC_CUTOFF = 15;

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

        if (SaveManager.Instance.Player.Day > COZY_MUSIC_CUTOFF)
        {
            AudioManager.Instance.PlayMusic("CozyMusic");
        }
        else
        {
            AudioManager.Instance.PlayMusic("HorrorMusic");
        }
        FoodGrab.CanMoveFood = true;
        ObjectGrab.CanMoveAppliances = false;
        Debug.Log("Phase: Cooking");
    }

    public void StartSetup()
    {
        CurrentPhase = GamePhase.Setup;
        AudioManager.Instance.PlayMusic("KitchenSetupMusic");
        FoodGrab.CanMoveFood = false;
        ObjectGrab.CanMoveAppliances = true;
        Debug.Log("Phase: Setup");
    }

}