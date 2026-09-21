using TMPro;
using UnityEngine;

public class SaveCard : MonoBehaviour
{
    private int _id;

    [SerializeField] private TextMeshProUGUI _dayText;

    public void SetData(SaveOverview overview)
    {
        _id = overview.ID;
        _dayText.text = overview.Day.ToString();
    }

    public void Click()
    {
        LoadSaveManager.Instance.Load(_id);
    }
}
