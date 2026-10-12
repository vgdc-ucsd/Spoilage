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

    private const string UNLUCKY_TWIN_BOY_ID = "Unlucky Twin Boy";
    private const string UNLUCKY_TWIN_GIRL_ID = "Unlucky Twin Girl";

    private Vector3 GuardOffset(float x, float y = 0f)
    {
        return _guardParent.TransformVector(new Vector3(x, y, 0f));
    }

    public void Init()
    {
        float resistance = SaveManager.Instance.Player.resistanceScore;
        if (resistance < 4) _totalGuardCalls = 7;
        else if (resistance < 9) _totalGuardCalls = 5;
        else _totalGuardCalls = 3;

        if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.GuardAllocation1))
            _totalGuardCalls += 1;
        else if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.GuardAllocation2))
            _totalGuardCalls += 2;
        else if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.GuardAllocation3))
            _totalGuardCalls += 3;

        _remainingGuardCalls = _totalGuardCalls;
        _guardsStaminaBar.SetStamina(1.0f);
    }

    public void Lock(bool locked)
    {
        _refusalButton.Lock(locked);
    }

    public bool RemoveCustomer()
    {
        if (_remainingGuardCalls == 0)
        {
            return false;
        }

        Customer currentCustomer =
            CustomerLineManager.Instance.CurrentCustomer;
        if (currentCustomer == null
            || currentCustomer.customerData == null
            || currentCustomer.customerData.spoilage == CustomerData.Spoilage.STAGE_II
            || currentCustomer.customerData.tier == CustomerData.Tier.Key)
        {
            return false;
        }

        SaveManager.Instance.Player.DayData.CustomersRefused++;
        _remainingGuardCalls--;

        _guardsStaminaBar.SetStamina(
            _remainingGuardCalls / (float)_totalGuardCalls
        );

        _refusalButton.Lock(true);

        if (currentCustomer.customerData.tier == CustomerData.Tier.SemiKey
            && !StoryManager.Instance.IsRejectedSemikey(
                currentCustomer.customerData))
        {
            SaveManager.Instance.Player.RejectedSemikeyCharacters.Add(
                currentCustomer.customerData.id
            );

            // rejecting one twin automatically rejects the other twin
            if (currentCustomer.customerData.id == UNLUCKY_TWIN_BOY_ID)
            {
                SaveManager.Instance.Player.RejectedSemikeyCharacters.Add(
                    UNLUCKY_TWIN_GIRL_ID
                );
            }
            else if (currentCustomer.customerData.id == UNLUCKY_TWIN_GIRL_ID)
            {
                SaveManager.Instance.Player.RejectedSemikeyCharacters.Add(
                    UNLUCKY_TWIN_BOY_ID
                );
            }
        }

        CustomerMovement rightGuard =
            Instantiate(_guardPrefab, _guardParent);

        CustomerMovement leftGuard =
            Instantiate(_guardPrefab, _guardParent);

        rightGuard.transform.localScale = Vector3.one * GUARD_SCALE;
        leftGuard.transform.localScale = Vector3.one * GUARD_SCALE;

        rightGuard.transform.position =
            _customerSpawnpoint.position
            + GuardOffset(0f, GUARD_HEIGHT_OFFSET);

        leftGuard.transform.position =
            _customerSpawnpoint.position
            + GuardOffset(-GUARD_DISTANCE, GUARD_HEIGHT_OFFSET);

        leftGuard.WalkTo(
            _customerOrderPoint.position + GuardOffset(-GUARD_DISTANCE),
            GUARD_WALK_DURATION,
            null
        );
        
        rightGuard.WalkTo(
            _customerOrderPoint.position + GuardOffset(GUARD_DISTANCE),
            GUARD_WALK_DURATION,
            () => InteractCustomer(currentCustomer, leftGuard, rightGuard)
        );

        return true;
    }

    private void InteractCustomer(
        Customer customer,
        CustomerMovement leftGuard,
        CustomerMovement rightGuard)
    {
        DialogueManager.Instance.PlayDialogue(
            customer.Dialogue.Reject,
            customer.customerData,
            () => TakeCustomer(customer, leftGuard, rightGuard)
        );
    }

    private void TakeCustomer(
        Customer customer,
        CustomerMovement leftGuard,
        CustomerMovement rightGuard)
    {
        leftGuard.WalkTo(
            _customerSpawnpoint.position
                + GuardOffset(-GUARD_DISTANCE * 2f),
            GUARD_WALK_DURATION,
            () => Destroy(leftGuard.gameObject)
        );

        customer.Movement.WalkTo(
            _customerSpawnpoint.position
                + GuardOffset(-GUARD_DISTANCE / 2f),
            GUARD_WALK_DURATION,
            () =>
            {
                _refusalButton.Lock(false);
                CustomerLineManager.Instance.Advance(true);
            }
        );

        rightGuard.WalkTo(
            _customerSpawnpoint.position,
            GUARD_WALK_DURATION,
            () => Destroy(rightGuard.gameObject)
        );
    }
}