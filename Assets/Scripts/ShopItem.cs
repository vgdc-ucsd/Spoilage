using UnityEngine;


[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop/Item")]
public class ShopItem : ScriptableObject
{
  public string itemName;
  public Sprite icon;
  public int price;
}
