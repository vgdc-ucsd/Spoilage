using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CustomerLineManager : Singleton<CustomerLineManager>
{
    public Customer CurrentCustomer;
    
    public UnityEvent PlateSubmitted;
    
    // TODO: Demo wiring, remove
    private bool _dayStarted;

    public void CallBellPressed()
    {
        // TODO: Demo wiring, remove
        StartDay();

        CheckOrder();
    }

    public void Advance()
    {
        StartCoroutine(LoadNextCustomerAnimation());
    }

    private void CheckOrder()
    {
        //sends a signal to the serving station to begin submission process
        //the actual order submission logic is in CustomerOrderDatabase
        //i <3 spaghetti code
        PlateSubmitted.Invoke();
    }

    // TODO: Demo wiring, remove
    private void StartDay()
    {
        if (_dayStarted) return;

        StartCoroutine(LoadNextCustomerAnimation());

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
