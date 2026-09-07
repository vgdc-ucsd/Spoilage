using System.Collections.Generic;
using UnityEngine;

public class ItemGeneratorScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShopItemScript _shopItemPrefab;
    [SerializeField] private List<ShopItem> _generalItemPool;
    [SerializeField] private List<ShopItem> _ingredientItemPool;
    [SerializeField] private Transform _itemsParent;

    [Header("Config")]
    [SerializeField] private int _generalUpgradeCount;
    [SerializeField] private int _ingredientUpgradeCount;

    void Start()
    {
        GenerateShopItems(_ingredientItemPool, _ingredientUpgradeCount);
        GenerateShopItems(_generalItemPool, _generalUpgradeCount);
    }

    private void GenerateShopItems(List<ShopItem> itemPool, int count)
    {
        for (int i = 0; i < count; i++)
        {
            ShopItem upgrade = itemPool[Random.Range(0, itemPool.Count)];

            ShopItemScript shopItem = Instantiate(_shopItemPrefab, _itemsParent);
            shopItem.item = upgrade;
        }
    }
}
