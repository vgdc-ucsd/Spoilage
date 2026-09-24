using UnityEngine;

public class LeaveShopButton : MonoBehaviour
{
    public void Click()
    {
        SaveManager.Instance.SaveToNew();
        GameManager.Instance.Load(GameScene.COOKING);
    }
}
