using UnityEngine;

public class LeaveShopButton : MonoBehaviour
{
    public void Click()
    {
        SaveManager.Instance.SaveToNew();
        
        if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.CookingInstant))
        {
            SaveManager.Instance.Player.DayData.RemainingInstantStationUses = 5;
        }
     
        GameManager.Instance.Load(GameScene.COOKING);
    }
}
