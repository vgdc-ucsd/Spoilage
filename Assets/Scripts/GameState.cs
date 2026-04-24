using System;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Scriptable Objects/GameState")]
public class GameState : ScriptableObject
{
    [NonSerialized]
    public TextMeshProUGUI moneyText = null;

    [SerializeField]
    private int _money;

    public int Money
    {
        get { return _money; }
        set
        {
            _money = value;
            UpdateUI();
            // If we have a master save manager, use instance.currentData.player.moneyGained
            if (SaveManager.Instance)
                SaveManager.Instance.currentData.moneyGained = _money;
            else
                Debug.LogError("No SaveManager!");
        }
    }

  private void UpdateUI()
  {
      if (moneyText)
          moneyText.text = "$" + Money.ToString();
  }

  private void OnValidate()
  {
      UpdateUI();
  }
}
