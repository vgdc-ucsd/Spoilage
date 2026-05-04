using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    private int _wealth;
    public int Wealth
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
    }

    public void LeaveShop()
    {
        SaveManager.Instance.Player.Wealth = _wealth;
    }
}
