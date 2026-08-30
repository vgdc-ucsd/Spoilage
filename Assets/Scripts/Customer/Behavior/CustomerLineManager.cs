using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TimeOfDay
{
    Beginning,
    Middle,
    End
}

public class CustomerLineManager : Singleton<CustomerLineManager>
{
    private TimeOfDay _timeOfDay;
    private Queue<Conversation> _beginningCustomers;
    private Queue<Conversation> _middleCustomers;
    private Queue<Conversation> _endCustomers;
    private Customer _customer;

    [SerializeField] private Transform _entrance;
    [SerializeField] private Transform _counter;
    [SerializeField] private Transform _exit;

    public void GenerateQueues()
    {
        InteractionSet interactions = ProgressionManager.Instance.GetInteractions();
        _beginningCustomers = new Queue<Conversation>(interactions.BeginInteractions.SelectMany(list => list));
        _middleCustomers = new Queue<Conversation>(interactions.MiddleInteractions.SelectMany(list => list));
        _endCustomers = new Queue<Conversation>(interactions.EndInteractions.SelectMany(list => list));
    }

    public void SetTime(TimeOfDay time)
    {
        _timeOfDay = time;
    }

    public void Advance()
    {
        if (_customer != null)
        {
            GameObject customerGameObject = _customer.gameObject;
            _customer.Movement.WalkTo(_exit.position, () => Destroy(customerGameObject));
        }

        CustomerData customerData;
        TextAsset conversationJson;

        if (_timeOfDay == TimeOfDay.Beginning)
        {
            if (_beginningCustomers.Count == 0)
            {
                SetupManager.Instance.StartSetupPhase();
                return;
            }

            Conversation conversation = _beginningCustomers.Dequeue();
            customerData = conversation.Customer;
            conversationJson = conversation.ConversationJson;
        }
        else if (_timeOfDay == TimeOfDay.Middle)
        {
            if (_middleCustomers.Count == 0)
            {
                conversationJson = null; // TODO
                customerData = CustomerManager.Instance.GenerateCustomerData();
                // DialogueManager.Instance.SelectGeneralDialogue(c);
            }
            else
            {   
                Conversation conversation = _middleCustomers.Dequeue();
                customerData = conversation.Customer;
                conversationJson = conversation.ConversationJson;
            }
        }
        else
        {
            if (_endCustomers.Count == 0)
            {
                SetupManager.Instance.FinishDay();
                return;
            }

            Conversation conversation = _endCustomers.Dequeue();
            customerData = conversation.Customer;
            conversationJson = conversation.ConversationJson;
        }

        CustomerDialogue dialogue = DialogueManager.Instance.LoadCustomerDialogue(conversationJson);
        _customer = CustomerManager.Instance.GenerateCustomer(customerData, dialogue);
        _customer.transform.position = new Vector3(_entrance.position.x, _customer.transform.position.y, _customer.transform.position.z);
        _customer.Movement.WalkTo(
            _counter.position,
            () => DialogueManager.Instance.PlayDialogue(dialogue.Intro, () => Advance())
        );
        // TODO check if the customer is a rejected semikey character that needs to be replaced
    }

   /*  public Customer CurrentCustomer;
    
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
    } */
}
