using System.Collections;
using UnityEngine;

public class CustomerLineManager : Singleton<CustomerLineManager>
{
    public Customer CurrentCustomer;

    public void CallBellPressed()
    {
        CheckOrder();
        StartCoroutine(LoadNextCustomerAnimation());
    }

    private void CheckOrder()
    {
        // TODO - check if order is correct
    }

    private IEnumerator UnloadCurrentCustomerAnimation()
    {
        if (CurrentCustomer == null) yield break;

        // TODO - customer slides out to left side

        Destroy(CurrentCustomer.gameObject);
        yield return null;
    }

    private IEnumerator LoadNextCustomerAnimation()
    {
        if (CurrentCustomer != null)
        {
            yield return StartCoroutine(UnloadCurrentCustomerAnimation());
        }
        CurrentCustomer = CustomerManager.Instance.GenerateCustomer();

        // TODO - customer slides in from left side

        yield return null;
    }
}
