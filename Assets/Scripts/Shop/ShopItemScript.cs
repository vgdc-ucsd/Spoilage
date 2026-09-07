using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    public Texture2D hoverCursor;
    public ShopItem item;
    [SerializeField] private TextMeshProUGUI _nameField;
    [SerializeField] private TextMeshProUGUI _typeField;
    [SerializeField] private TextMeshProUGUI _priceField;
    [SerializeField] private Image _cardImage;
    [SerializeField] private Image _itemImage;    

    private bool _bought = false;
    private Vector3 _origScale;

    void Start()
    {
        _origScale = transform.localScale;
        UpdateGUI();
    }

    bool CanBuy()
    {
        return !_bought && SaveManager.Instance.Player.Wealth >= item.price;
    }

    public void UpdateGUI()
    {
        _priceField.text = "$" + item.price;
        _nameField.text = item.name;
        _typeField.text = $"- {item.itemType} -";
        _itemImage.sprite = item.icon;
        _cardImage.color = item.color;
        
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
            transform.localScale *= 1.1f;
    
        // if (CanBuy())
            // Cursor.SetCursor(hoverCursor, new Vector2(hoverCursor.width, hoverCursor.height) / 2, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        transform.localScale = _origScale;
        
        // Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CanBuy())
        {
            ShopManager.Instance.BuyItem(item);
            _bought = true;

            UpdateGUI();
            transform.localScale = _origScale;
            // Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
