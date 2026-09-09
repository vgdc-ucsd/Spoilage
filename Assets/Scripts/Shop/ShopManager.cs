using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    public void BuyItem(Upgrade upgrade)
    {
        // TODO: REMOVE ITEM FROM SHOP POOL IF IT'S A ONE-TIME PURCHASE
        SaveManager.Instance.Player.Wealth -= upgrade.Cost;
        ProgressionManager.Instance.Purchased.Add(upgrade.UpgradeID);
    }
}
