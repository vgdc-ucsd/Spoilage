using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] private GiveItemTileUI _giveItemUI;
    [SerializeField] private List<ItemData> _items;

    void Start()
    {
        _giveItemUI.Hide();
    }

    private ItemData FindItem(string id)
    {
        return _items.Find(item => item.ID == id);
    }

    public void GiveItem(DialogueItemData itemData)
    {
        _giveItemUI.Show(FindItem(itemData.ID));
        SetupManager.Instance.LockKitchenTiles(false);
    }
}
