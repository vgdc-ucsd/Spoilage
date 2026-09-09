using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    public Texture2D hoverCursor;
    public Upgrade upgrade;
    [SerializeField] private TextMeshProUGUI _nameField;
    [SerializeField] private TextMeshProUGUI _typeField;
    [SerializeField] private TextMeshProUGUI _descriptionField;
    [SerializeField] private TextMeshProUGUI _priceField;
    [SerializeField] private Image _cardImage;
    [SerializeField] private Image _itemImage;    

    private bool _bought = false;
    private Vector3 _origScale;

    void Start()
    {
        _origScale = transform.localScale;
        _descriptionField.gameObject.SetActive(false);
        UpdateGUI();
    }

    bool CanBuy()
    {
        return !_bought && SaveManager.Instance.Player.Wealth >= upgrade.Cost;
    }

    public void UpdateGUI()
    {
        _priceField.text = "$" + upgrade.Cost;
        _nameField.text = upgrade.Name;
        _typeField.text = $"- {upgrade.UpgradeType} -";
        _descriptionField.text = $"{upgrade.Description}";
        _itemImage.sprite = upgrade.Icon;
        _cardImage.color = upgrade.Color;
        
        if (_bought)
        {
            _cardImage.color *= Color.gray;
            _itemImage.color *= Color.gray;
            _priceField.color *= Color.gray;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_bought)
        {
            transform.localScale *= 1.1f;
            FlipCard();
        }

        // if (CanBuy())
            // Cursor.SetCursor(hoverCursor, new Vector2(hoverCursor.width, hoverCursor.height) / 2, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        transform.localScale = _origScale;
        
        if (!_bought) 
            FlipCard();

        // Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CanBuy())
        {
            ShopManager.Instance.BuyItem(upgrade);
            _bought = true;

            UpdateGUI();
            transform.localScale = _origScale;
            FlipCard();
            // Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void FlipCard()
    {
        // don't show the description if it's an ingredient upgrade because there are no descriptions for ingredients
        if (upgrade.UpgradeType == UpgradeType.Ingredient) return;

        _descriptionField.gameObject.SetActive(!_descriptionField.gameObject.activeSelf);
        _typeField.gameObject.SetActive(!_typeField.gameObject.activeSelf);
        _itemImage.gameObject.SetActive(!_itemImage.gameObject.activeSelf);
    }
}
