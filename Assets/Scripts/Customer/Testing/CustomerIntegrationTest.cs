using System.Collections;
using UnityEngine;

public class CustomerIntegrationTest : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TestCustomerOnLoad());
    }

    IEnumerator TestCustomerOnLoad()
    {
        while (!CustomerOrderDatabase.Instance.Loaded)
        {
            yield return null;
        }

        TestCustomer();
    }

    private void TestCustomer()
    {
        Customer customer = CustomerManager.Instance.GenerateCustomer();
        CustomerDialogue dialogue = DialogueManager.Instance.SelectGeneralDialogue(customer.customerData);
        DialogueManager.Instance.PlayDialogue(dialogue.Intro, null);
    }
}
