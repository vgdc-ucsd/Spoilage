using System.Collections;
using UnityEngine;

public class CustomerLineManager : Singleton<CustomerLineManager>
{
    public Customer CurrentCustomer;
    
    // TODO: Demo wiring, remove
    private bool _dayStarted;

    private KitchenTile _servingArea;

    public void CallBellPressed()
    {
        if (_servingArea == null)
        {
            _servingArea = GameObject.FindWithTag("Serving Area Tile").GetComponent<KitchenTile>();
        }

        // TODO: Demo wiring, remove
        StartDay();

        GameObject itemToBeServed = _servingArea.GetTopObject();
        if (itemToBeServed == null) return;

        if (itemToBeServed.TryGetComponent(out IngredientObject foodItem))
        {
            // if it is a food item, check if it matches the current customer's order
            CheckOrder();

            // TODO: check if the customer has no more pending food items in their order
            // if so, they should leave and the next customer should come up
            StartCoroutine(LoadNextCustomerAnimation());
        }

        if (itemToBeServed.TryGetComponent(out StoryItemObject storyItem))
        {
            // if it is a story item, check if the current customer wants it
            TryGiveStoryItem(storyItem);
        }
    }

    public void Advance()
    {
        StartCoroutine(LoadNextCustomerAnimation());
    }

    private void CheckOrder()
    {
        // TODO - check if order is correct
    }

    private void TryGiveStoryItem(StoryItemObject storyItem)
    {
        if (CurrentCustomer.customerData.desiredStoryItem == null || 
                CurrentCustomer.customerData.desiredStoryItem != storyItem.StoryItemInstance.Data)
        {
            // Customer does not want this story item
            // TODO: play story item rejection dialogue
            Debug.Log("Customer does not want this story item");
        }
        else
        {
            // Customer wants this story item
            // TODO: play story item acceptance dialogue
            // TODO: destroy story item object
            // TODO: set story item as received in story manager
            Debug.Log("Customer wants this story item");
        }
    }

    // TODO: Demo wiring, remove
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
