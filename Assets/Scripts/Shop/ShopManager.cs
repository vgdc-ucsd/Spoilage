using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private UpgradeShopSpawner _upgradeSpawner;

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

        // 1. Wipe last session's temporary upgrades so the filter below sees clean state
        UpgradeManager.ClearTemporaryUpgrades();

        // 2. Spawn the three upgrade slots with available (unowned) upgrades
        if (_upgradeSpawner != null)
            _upgradeSpawner.SpawnUpgrades();
        else
            Debug.LogWarning("ShopManager: _upgradeSpawner is not assigned — no upgrade items will appear.");
    }

    public void LeaveShop()
    {
        SaveManager.Instance.Player.Wealth = _wealth;
    }
}
