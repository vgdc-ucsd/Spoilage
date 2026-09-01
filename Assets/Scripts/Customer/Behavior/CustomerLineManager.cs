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
    public Customer CurrentCustomer => _customer;

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
            _customer.Movement.WalkTo(_exit.position, 6f, () => Destroy(customerGameObject));
        }

        CustomerData customerData;
        CustomerDialogue dialogue;

        if (_timeOfDay == TimeOfDay.Beginning)
        {
            if (_beginningCustomers.Count == 0)
            {
                SetupManager.Instance.StartSetupPhase();
                return;
            }

            Conversation conversation = _beginningCustomers.Dequeue();
            customerData = conversation.Customer;
            dialogue = DialogueManager.Instance.LoadCustomerDialogue(conversation.ConversationJson);
        }
        else if (_timeOfDay == TimeOfDay.Middle)
        {
            if (_middleCustomers.Count == 0)
            {
                customerData = CustomerManager.Instance.GenerateCustomerData();
                dialogue = DialogueManager.Instance.SelectGeneralDialogue(customerData);
            }
            else
            {   
                Conversation conversation = _middleCustomers.Dequeue();
                customerData = conversation.Customer;
                dialogue = DialogueManager.Instance.LoadCustomerDialogue(conversation.ConversationJson);
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
            dialogue = DialogueManager.Instance.LoadCustomerDialogue(conversation.ConversationJson);
        }

        _customer = CustomerManager.Instance.GenerateCustomer(customerData);
        _customer.transform.position = new Vector3(
            _entrance.position.x, 
            _customer.transform.position.y, 
            _customer.transform.position.z
        );

        _customer.Movement.WalkTo(
            _counter.position,
            2f,
            () => DialogueManager.Instance.PlayDialogue(
                dialogue.Intro, 
                () => {
                    if (_timeOfDay != TimeOfDay.Middle) Advance();
                }
            )
        );

        // TODO check if the customer is a rejected semikey character that needs to be replaced
    }
}
