using TMPro;
using UnityEngine;

public class ShopUI : Singleton<ShopUI>
{
    [SerializeField] private TextMeshProUGUI _wealthText;

    void Update()
    {
        _wealthText.text = $"${SaveManager.Instance.Player.Wealth}";
    }
}
