using TMPro;
using UnityEngine;

public class ShopUI : Singleton<ShopUI>
{
    [SerializeField] private TextMeshProUGUI _wealthText;

    void Start()
    {
        AudioManager.Instance.PlayMusicEntry("Shop");
    }
    void Update()
    {
        _wealthText.text = $"${SaveManager.Instance.Player.Wealth}";
    }
}
