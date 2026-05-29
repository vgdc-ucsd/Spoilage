using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CustomerLineManager : Singleton<CustomerLineManager>
{
    public Customer CurrentCustomer;
    
    public UnityEvent PlateSubmitted;

    [SerializeField] private CustomerData _debug_warlordData;
    
    // TODO: Demo wiring, remove
    private bool _dayStarted;

    public void CallBellPressed()
    {
        // TODO: Demo wiring, remove
        if (!_dayStarted)
        {
            StartDay();
        }
        else
        {
            CheckOrder();
        }
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

        if (CurrentCustomer.customerData.orders.Count == 0)
        {
            Debug.LogWarning("Current customer has no orders! Skipping order check.");
            Advance();
            return;
        }

        PlateSubmitted.Invoke();
    }

    // TODO: Demo wiring, remove
    private void StartDay()
    {
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

        CustomerDialogue dialogue;
        if (CurrentCustomer.customerData == _debug_warlordData)
        {
            Debug.Log("DEBUG: Loading warlord dialogue.");
            dialogue = DialogueManager.Instance.DEBUGLoadWarlordDialogue();
        }
        else
        {
            dialogue = DialogueManager.Instance.SelectGeneralDialogue(CurrentCustomer.customerData);
        }
        DialogueManager.Instance.PlayDialogue(dialogue.Intro, null);

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
