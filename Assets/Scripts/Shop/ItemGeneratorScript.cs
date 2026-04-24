using System.Collections.Generic;
using UnityEngine;

public class ItemGeneratorScript : MonoBehaviour
{
    public GameObject shopItemPrefab;

    public List<ShopItem> itemPool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // pick from itemPool
        ShopItem item = itemPool[Random.Range(0, itemPool.Count)];

        // create shop item
        GameObject shopItem = Instantiate(shopItemPrefab);
        shopItem.GetComponent<SpriteRenderer>().sprite = item.icon;
        shopItem.GetComponent<ShopItemScript>().price = item.price;
        shopItem.GetComponent<ShopItemScript>().UpdatePrice();
        shopItem.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
