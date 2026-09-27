using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] private List<GiveItemTileUI> _giveItemUis;
    [SerializeField] private List<ItemData> _items;

    void Start()
    {
        foreach (GiveItemTileUI giveItemUI in _giveItemUis)
        {
            giveItemUI.Hide();
        }
    }

    private ItemData FindItem(string id)
    {
        return _items.Find(item => item.ID == id);
    }

    public void GiveItems(List<DialogueItemData> itemData)
    {
        for (int i = 0; i < _giveItemUis.Count; i++)
        {
            if (i < itemData.Count) _giveItemUis[i].Show(FindItem(itemData[i].ID));
            else _giveItemUis[i].Hide();
        }
        SetupManager.Instance.LockKitchenTiles(false);
    }
}
