using System.Collections.Generic;
using UnityEngine;

public class ItemGeneratorScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShopItemScript _shopItemPrefab;
    [SerializeField] private List<Upgrade> _generalItemPool;
    [SerializeField] private List<Upgrade> _ingredientItemPool;
    [SerializeField] private Transform _itemsParent;

    [Header("Config")]
    [SerializeField] private int _generalUpgradeCount;
    [SerializeField] private int _ingredientUpgradeCount;


    [Header("Debug")]
    [SerializeField] private bool _refresh;

    void Start()
    {
        // _generalItemPool = ProgressionManager.Instance.ShopPool;
        foreach (UpgradeID id in ProgressionManager.Instance.ShopPool)
        {
            Upgrade upgrade = ProgressionManager.Instance.Upgrades[id];

            if (upgrade.UpgradeType == UpgradeType.Ingredient)
                _ingredientItemPool.Add(upgrade);
            else
                _generalItemPool.Add(upgrade);
        }

        GenerateShopItems(_ingredientItemPool, _ingredientUpgradeCount);
        GenerateShopItems(_generalItemPool, _generalUpgradeCount);
    }

    void Update()
    {
        if (_refresh)
        {
            _refresh = false;
            for (int i = 0; i < _itemsParent.childCount; i++)
            {
                Destroy(_itemsParent.GetChild(i).gameObject);
            }

            GenerateShopItems(_ingredientItemPool, _ingredientUpgradeCount);
            GenerateShopItems(_generalItemPool, _generalUpgradeCount);   
        }
    }

    private void GenerateShopItems(List<Upgrade> itemPool, int count)
    {
        if (itemPool.Count == 0) 
        {
            Debug.LogWarning("Item Pool is empty");
            return;
        }
        
        for (int i = 0; i < count; i++)
        {
            // TODO: FIX THIS -- SHOULD NOT BE PURELY RANDOM BECAUSE THIS CAN CAUSE DUPLICATE UPGRADES
            Upgrade upgrade = itemPool[Random.Range(0, itemPool.Count)];

            ShopItemScript shopItem = Instantiate(_shopItemPrefab, _itemsParent);
            shopItem.upgrade = upgrade;
        }
    }
}
