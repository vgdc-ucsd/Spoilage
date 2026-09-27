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

        if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.AutoCuttingBoard))
        {
            if (SaveManager.Instance.Player.KitchenStations.Contains("Cutting Board"))
            {
                SaveManager.Instance.Player.KitchenStations.Remove("Cutting Board");
                SaveManager.Instance.Player.KitchenStations.Add("Slicer");
            }
        }

        if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.AutoBlender))
        {
            if (SaveManager.Instance.Player.KitchenStations.Contains("Blender"))
            {
                SaveManager.Instance.Player.KitchenStations.Remove("Blender");
                SaveManager.Instance.Player.KitchenStations.Add("Food Processor");
            }
        }

        GameManager.Instance.Load(GameScene.COOKING);
    }
}
