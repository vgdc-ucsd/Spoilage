using UnityEngine;

public class NextDayButton : MonoBehaviour
{
    public void NextDay()
    {
        // TODO advance and save day
        GameManager.Instance.Load(GameScene.SHOP);
    }
}
