using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    // private int _wealth;
    // public int Wealth
    // {
    //     get => _wealth;
    //     set
    //     {
    //         ShopUI.Instance.SetWealth(value);
    //         _wealth = value;
    //     }
    // }

    void Start()
    {
        // TODO: REMOVE THIS (THIS IS JUST FOR TESTING)
        SaveManager.Instance.Player.Wealth += 100;

        // SaveManager.OnPlayerLoad(() => InitializeShop());
    }

    // private void InitializeShop()
    // {
    //     Wealth = SaveManager.Instance.Player.Wealth;

    //     Wealth += 100;
    // }

    public void BuyItem(ShopItem item)
    {
        SaveManager.Instance.Player.Wealth -= item.price;
    }

    // public void LeaveShop()
    // {
    //     SaveManager.Instance.Player.Wealth = _wealth;
    // }
}
