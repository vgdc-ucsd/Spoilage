using UnityEngine;
using TMPro;
using System;

public class SummaryStats : MonoBehaviour
{
    [SerializeField] private StarRatingSystem _stars;
    [SerializeField] private TextMeshProUGUI _profits;
    [SerializeField] private TextMeshProUGUI _tax;
    [SerializeField] private TextMeshProUGUI _rent;
    [SerializeField] private TextMeshProUGUI _bank;
    [SerializeField] private TextMeshProUGUI _total;
    [SerializeField] private TextMeshProUGUI _calendarDay;
    [SerializeField] private TextMeshProUGUI _calendarMonth;
    [SerializeField] private TextMeshProUGUI _customersServed;
    [SerializeField] private TextMeshProUGUI _customersRefused;

    private const int RENT = -100;
    private const string MONTH = "NOV";

    public void Start()
    {
        SaveManager.OnPlayerLoad(CalculateStats);
    }

    public void CalculateStats()
    {
        PlayerData player = SaveManager.Instance.Player;
        int tax = CalculateTax();

        // TODO
        // float starRating = player.Reputation;
        // _stars.UpdateStarRating();

        _calendarMonth.text = MONTH;
        _calendarDay.text = $"{player.Day}";
        _customersServed.text = $"{new string('x', player.DayData.CustomersServed)}";
        _customersRefused.text = $"{new string('x', player.DayData.CustomersRefused)}";
        _profits.text = $"${player.DayData.Profits}";
        _rent.text = $"${RENT}";
        _tax.text = $"${tax}";
        _bank.text = $"${player.Wealth}";

        int total = player.Wealth + player.DayData.Profits + RENT + tax;
        _total.text = $"${total}";
        player.Wealth = total;
    }

    public int CalculateTax()
    {
        float rate = SaveManager.Instance.Player.resistanceScore < 4f ? 0.1f : 0.2f;
        return -Mathf.FloorToInt(rate * SaveManager.Instance.Player.DayData.Profits);
    }
}
