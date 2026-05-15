using System.Collections;
using UnityEngine;

public class CustomerLineManager : Singleton<CustomerLineManager>
{
    public Customer CurrentCustomer;
    private bool _dayStarted;

    public void CallBellPressed()
    {
        StartDay();
        CheckOrder();
        StartCoroutine(LoadNextCustomerAnimation());
    }

    public void Advance()
    {
        StartCoroutine(LoadNextCustomerAnimation());
    }

    private void CheckOrder()
    {
        // TODO - check if order is correct
    }

    private void StartDay()
    {
        if (_dayStarted) return;

        StoryManager.Instance.InitRun();
        StoryManager.Instance.BeginDay();
        _dayStarted = true;
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
        CurrentCustomer = GenerateCustomer();

        // TODO - customer slides in from left side

        yield return null;
    }

    private Customer GenerateCustomer()
    {
        if (!StoryManager.Instance.TryDequeueCustomer(out CustomerData customerData))
        {
            return CustomerManager.Instance.GenerateCustomer();
        }

        return customerData == null
            ? CustomerManager.Instance.GenerateCustomer()
            : CustomerManager.Instance.GenerateCustomer(customerData);
    }
}
