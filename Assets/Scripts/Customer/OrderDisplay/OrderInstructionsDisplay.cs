using TMPro;
using UnityEngine;

public class OrderInstructionsDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _instructionsText;

    [SerializeField]
    private RecipeManager _recipeManager;

    private void Awake()
    {
        if (_recipeManager == null)
        {
            _recipeManager = FindAnyObjectByType<RecipeManager>();
        }
    }

    public void DisplayCustomer(Customer customer)
    {
        if (_instructionsText == null)
        {
            return;
        }

        if (customer == null || customer.customerData == null)
        {
            Clear();
            return;
        }

        _instructionsText.text = OrderInstructionFormatter.FormatOrders(customer.customerData.orders, _recipeManager);
    }

    public void Clear()
    {
        if (_instructionsText != null)
        {
            _instructionsText.text = string.Empty;
        }
    }
}
