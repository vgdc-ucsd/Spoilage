using UnityEngine;
using TMPro;

public class SummaryStats : MonoBehaviour
{
    [SerializeField] private StarRatingSystem _stars;
    [SerializeField] private TextMeshProUGUI _profits;
    [SerializeField] private TextMeshProUGUI _expenses;
    [SerializeField] private TextMeshProUGUI _rent;
    [SerializeField] private TextMeshProUGUI _spent;
    [SerializeField] private TextMeshProUGUI _total;
    [SerializeField] private TextMeshProUGUI _calendarDay;
    [SerializeField] private TextMeshProUGUI _calendarMonth;
    [SerializeField] private TextMeshProUGUI _customersServed;
    [SerializeField] private TextMeshProUGUI _customersRefused;

    public void Start()
    {
        SaveManager.OnPlayerLoad(CalculateStats);
    }

    public void CalculateStats()
    {

    }
}
