public class ShopManager : Singleton<ShopManager>
{
    public void BuyItem(Upgrade upgrade)
    {
        AudioManager.Instance.PlaySFX("ShopPurchase");
        SaveManager.Instance.Player.Wealth -= upgrade.Cost;
        ProgressionManager.Instance.Purchase(upgrade);
        
        if (!upgrade.Temporary)
        {
            ProgressionManager.Instance.ShopPool.Remove(upgrade.UpgradeID);
        }
    }
}
