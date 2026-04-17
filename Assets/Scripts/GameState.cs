using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GameState", menuName = "Scriptable Objects/GameState")]
public class GameState : ScriptableObject
{
  public Text moneyText;

  private int _money;

  public int Money
  {
    get { return _money; }
    set
    {
      _money = value;
      UpdateUI();
    }
  }

  private void UpdateUI()
  {
    moneyText.text = "$" + Money.ToString();
  }
}
