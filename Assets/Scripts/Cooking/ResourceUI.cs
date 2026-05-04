using UnityEngine;
using TMPro;

public class ResourceUI : Singleton<ResourceUI>
{
    [SerializeField] TextMeshProUGUI _wealthText;

    public void UpdateWealth(int wealth)
    {
        _wealthText.text = "$" + wealth.ToString();
    }

    public void UpdateReputation(int reputation)
    {
        // TODO
    }
}
