using UnityEngine;

public class NextDayButton : MonoBehaviour
{
    public void NextDay()
    {
        ProgressionManager.Instance.AdvanceDay();
        GameManager.Instance.Load(GameScene.SHOP);
    }
}
