using UnityEngine;
using TMPro;

public class SummaryStats : MonoBehaviour
{
    private PlayerData _playerData;
    private int _rentCost = 50; // Constant $50 according to Ethan Carter as of 5/24/2026

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private TextMeshProUGUI _customerServedText;
    [SerializeField] private TextMeshProUGUI _customerRefusedText;
    [SerializeField] private TextMeshProUGUI _revenueText;
    [SerializeField] private TextMeshProUGUI _expensesText;
    [SerializeField] private TextMeshProUGUI _rentText;
    [SerializeField] private TextMeshProUGUI _profitsText;
    [SerializeField] private TextMeshProUGUI _totalText;

    void Start()
    {
        _playerData = SaveManager.Instance.Player;
        
        UpdateUIText();
    }

    void UpdateUIText()
    {
        _dayText.text = $"{_playerData.Day}";
        _customerServedText.text = $"{_playerData.CurrentDayCustomersServed}";
        _customerRefusedText.text = $"{_playerData.CurrentDayCustomersRefused}";

        _revenueText.text = $"${(int)_playerData.Revenue}";
        _expensesText.text = $"${(int)_playerData.Expenses}";
        _rentText.text = $"${_rentCost}";

        float profit = _playerData.Revenue - _playerData.Expenses - _rentCost;
        _profitsText.text = profit >= 0 ? $"${(int)profit}" : $"-${(int)-profit}";

        _totalText.text = $"${_playerData.Wealth}";
    }

    public void NextDayButton()
    {
        Debug.Log("TODO: Load next scene");
    }
}
