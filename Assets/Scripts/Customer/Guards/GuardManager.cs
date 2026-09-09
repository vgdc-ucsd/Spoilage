using UnityEngine;

public class GuardManager : Singleton<GuardManager>
{
    [SerializeField] private GuardsStaminaBar _guardsStaminaBar;
    [SerializeField] private RefusalButton _refusalButton;
    [SerializeField] private CustomerMovement _guardPrefab;
    [SerializeField] private Transform _customerSpawnpoint;
    [SerializeField] private Transform _customerOrderPoint;
    [SerializeField] private Transform _guardParent;

    private int _remainingGuardCalls;
    private int _totalGuardCalls;
    private const float GUARD_DISTANCE = 250f;
    private const float GUARD_WALK_DURATION = 1.5f;
    private const float GUARD_SCALE = 0.594f;
    private const float GUARD_HEIGHT_OFFSET = 100f;

    public void Init()
    {
        float resistance = SaveManager.Instance.Player.resistanceScore;
        if (resistance < 4) _totalGuardCalls = 7;
        else if (resistance < 9) _totalGuardCalls = 5;
        else _totalGuardCalls = 3;
        _remainingGuardCalls = _totalGuardCalls;
        _guardsStaminaBar.SetStamina(1.0f);
    }

    public void Lock(bool locked)
    {
        _refusalButton.Lock(locked);
    }

    public void RemoveCustomer()
    {
        if (_remainingGuardCalls == 0)
        {
            // TODO
            return;
        }

        Customer currentCustomer = CustomerLineManager.Instance.CurrentCustomer;
        SaveManager.Instance.Player.DayData.CustomersRefused++;
        _remainingGuardCalls--;
        _guardsStaminaBar.SetStamina(_remainingGuardCalls/(float)_totalGuardCalls);
        _refusalButton.Lock(true);

        Vector3 heightOffset = Vector3.up * GUARD_HEIGHT_OFFSET;
        CustomerMovement rightGuard = Instantiate(_guardPrefab, _guardParent);
        CustomerMovement leftGuard = Instantiate(_guardPrefab, _guardParent);
        rightGuard.transform.position = _customerSpawnpoint.position + heightOffset;
        leftGuard.transform.position = _customerSpawnpoint.position + (Vector3.left * GUARD_DISTANCE) + heightOffset;
        rightGuard.transform.localScale = Vector3.one * GUARD_SCALE;
        leftGuard.transform.localScale = Vector3.one * GUARD_SCALE;

        leftGuard.WalkTo(
            _customerOrderPoint.transform.position + (Vector3.left * (GUARD_DISTANCE / 2f)),
            GUARD_WALK_DURATION,
            null
        );

        rightGuard.WalkTo(
            _customerOrderPoint.transform.position + (Vector3.right * (GUARD_DISTANCE / 2f)),
            GUARD_WALK_DURATION,
            () => InteractCustomer(currentCustomer, leftGuard, rightGuard)
        );
    }

    private void InteractCustomer(Customer customer, CustomerMovement leftGuard, CustomerMovement rightGuard)
    {
        DialogueManager.Instance.PlayDialogue(
            customer.Dialogue.Reject,
            customer.customerData,
            () => TakeCustomer(customer, leftGuard, rightGuard)
        );
    }

    private void TakeCustomer(Customer customer, CustomerMovement leftGuard, CustomerMovement rightGuard)
    {
        leftGuard.WalkTo(
            _customerSpawnpoint.transform.position + (Vector3.left * GUARD_DISTANCE),
            GUARD_WALK_DURATION,
            () => Destroy(leftGuard.gameObject)
        );

        customer.Movement.WalkTo(
            _customerSpawnpoint.transform.position + (Vector3.left * (GUARD_DISTANCE / 2f)),
            GUARD_WALK_DURATION,
            () =>
            {
                _refusalButton.Lock(false);
                CustomerLineManager.Instance.Advance(true);
            }
        );

        rightGuard.WalkTo(
            _customerSpawnpoint.transform.position,
            GUARD_WALK_DURATION,
            () => Destroy(rightGuard.gameObject)
        );
    }
}
