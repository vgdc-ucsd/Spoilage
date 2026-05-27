using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    private float _wealth;
    public float Wealth
    {
        get => _wealth;
        set
        {
            ShopUI.Instance.SetWealth(value);
            _wealth = value;
        }
    }

    void Start()
    {
        SaveManager.OnLoad(() => InitializeShop());
    }

    private void InitializeShop()
    {
        Wealth = SaveManager.Instance.Player.Wealth;
        // Temporary upgrades expire at the end of each kitchen day (i.e., on shop entry)
        UpgradeManager.ClearTemporaryUpgrades();
    }

    public void LeaveShop()
    {
        SaveManager.Instance.Player.Wealth = _wealth;
    }
}
