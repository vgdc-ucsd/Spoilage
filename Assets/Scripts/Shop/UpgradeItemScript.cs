using TMPro;
using UnityEngine;

public class UpgradeItemScript : MonoBehaviour
{
    public Texture2D hoverCursor;
    public UpgradeData upgrade;

    public TextMeshPro priceField;
    public TextMeshPro nameField;
    public TextMeshPro typeField;
    public SpriteRenderer imageField;

    private bool _bought = false;

    void Start()
    {
        _bought = SaveManager.Instance.Player.Upgrades.Contains(upgrade.upgradeID);
        UpdateGUI();
    }

    private bool CanBuy()
    {
        return !_bought && ShopManager.Instance.Wealth >= upgrade.price;
    }

    void OnMouseDown()
    {
        if (!CanBuy()) return;

        ShopManager.Instance.Wealth -= upgrade.price;
        SaveManager.Instance.Player.Upgrades.Add(upgrade.upgradeID);
        _bought = true;
        GetComponent<SpriteRenderer>().color *= Color.gray;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void OnMouseEnter()
    {
        if (CanBuy())
            Cursor.SetCursor(hoverCursor, new Vector2(hoverCursor.width, hoverCursor.height) / 2, CursorMode.Auto);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void UpdateGUI()
    {
        priceField.text = "$" + upgrade.price;
        nameField.text = upgrade.upgradeName;
        typeField.text = "- " + upgrade.upgradeType.ToString() + " -";
        imageField.sprite = upgrade.icon;
        GetComponent<SpriteRenderer>().color = upgrade.color;
        if (_bought)
            GetComponent<SpriteRenderer>().color *= Color.gray;
    }
}
