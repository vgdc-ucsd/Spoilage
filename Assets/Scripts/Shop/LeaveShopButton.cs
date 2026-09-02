using UnityEngine;

public class LeaveShopButton : MonoBehaviour
{
    public void Click()
    {
        GameManager.Instance.Load(GameScene.COOKING);
    }
}
