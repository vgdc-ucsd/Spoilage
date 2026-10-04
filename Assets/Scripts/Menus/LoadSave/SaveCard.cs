using TMPro;
using UnityEngine;

public class SaveCard : MonoBehaviour
{
    private int _id;

    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private TextMeshProUGUI _wealthText;
    [SerializeField] private TextMeshProUGUI _ingredientsText;
    [SerializeField] private TextMeshProUGUI _stationsText;

    public void SetData(SaveOverview overview)
    {
        _id = overview.ID;
        _dayText.text = $"Day {overview.Day}";
        _wealthText.text = $"${overview.Wealth}";
        _ingredientsText.text = $"x{overview.IngredientsUnlocked}";
        _stationsText.text = $"x{overview.StationsUnlocked}";
    }

    public void Click()
    {
        LoadSaveManager.Instance.Load(_id);
    }
}
