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
        if (upgrade == null) return; // spawned at runtime — Initialize() will configure
        _bought = IsAlreadyOwned();
        UpdateGUI();
    }

    /// <summary>
    /// Called by UpgradeShopSpawner immediately after Instantiate.
    /// Sets the upgrade and refreshes the display.
    /// </summary>
    public void Initialize(UpgradeData upgradeData)
    {
        upgrade = upgradeData;
        _bought = false; // spawner already filtered out owned upgrades
        UpdateGUI();
    }

    private bool IsAlreadyOwned()
    {
        PlayerData p = SaveManager.Instance.Player;
        return p.Upgrades.Contains(upgrade.upgradeID) ||
               p.TemporaryUpgrades.Contains(upgrade.upgradeID);
    }

    private bool PrerequisitesMet()
    {
        if (upgrade.prerequisites == null || upgrade.prerequisites.Length == 0) return true;
        PlayerData p = SaveManager.Instance.Player;
        foreach (string prereqID in upgrade.prerequisites)
        {
            if (!p.Upgrades.Contains(prereqID) && !p.TemporaryUpgrades.Contains(prereqID))
                return false;
        }
        return true;
    }

    private bool CanBuy()
    {
        return !_bought && PrerequisitesMet() && ShopManager.Instance.Wealth >= upgrade.price;
    }

    void OnMouseDown()
    {
        if (!CanBuy()) return;

        ShopManager.Instance.Wealth -= upgrade.price;

        PlayerData p = SaveManager.Instance.Player;
        if (upgrade.isTemporary)
            p.TemporaryUpgrades.Add(upgrade.upgradeID);
        else
            p.Upgrades.Add(upgrade.upgradeID);

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
        typeField.text = "- " + (upgrade.isTemporary ? "One Day" : upgrade.upgradeType.ToString()) + " -";
        imageField.sprite = upgrade.icon;
        GetComponent<SpriteRenderer>().color = upgrade.color;
        if (_bought)
            GetComponent<SpriteRenderer>().color *= Color.gray;
    }
}
