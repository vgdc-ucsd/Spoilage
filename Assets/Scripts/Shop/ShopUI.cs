using TMPro;
using UnityEngine;

public class ShopUI : Singleton<ShopUI>
{
    [SerializeField] private TextMeshProUGUI _wealthText;

    public void SetWealth(float wealth)
    {
        _wealthText.text = $"${wealth}";
    }
}
