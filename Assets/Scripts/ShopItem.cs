using UnityEngine;


[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop/Item")]
public class ShopItem : ScriptableObject
{
  public string itemName;
  public string itemType;
  public Sprite icon;
  public int price;

  public Color color;
}
