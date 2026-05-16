using UnityEngine;

public class CustomerGenerationDemo : MonoBehaviour
{
    [SerializeField] private int _numCustomers;
    [SerializeField] private Transform _customerContainer;

    void Start()
    {
        for (int i = 0; i < _numCustomers; i++)
        {
            Customer customer = CustomerManager.Instance.GenerateCustomer();
            customer.transform.SetParent(_customerContainer, false);
            customer.transform.localScale = Vector3.one * 0.5f;
        }
    }

    public void Regenerate()
    {
        foreach (Transform child in _customerContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _numCustomers; i++)
        {
            Customer customer = CustomerManager.Instance.GenerateCustomer();
            customer.transform.SetParent(_customerContainer, false);
            customer.transform.localScale = Vector3.one * 0.5f;
        }
    }
}
