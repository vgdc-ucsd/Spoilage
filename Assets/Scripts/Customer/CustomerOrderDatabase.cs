using System.Collections.Generic;
using UnityEngine;

public class CustomerOrderDatabase : MonoBehaviour
{
    public static CustomerOrderDatabase Instance => s_instance;

    private static CustomerOrderDatabase s_instance;

    [SerializeField]
    private CustomerOrder[] _customerOrders;

    [Header("Chance curves based on game progress from 0 to 1")]
    [SerializeField]
    private AnimationCurve _oneDishChance;

    [SerializeField]
    private AnimationCurve _twoDishChance;

    [SerializeField]
    private AnimationCurve _threeDishChance;

    [SerializeField]
    private AnimationCurve _fourDishChance;

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(this);
            return;
        }

        s_instance = this;
    }

    public int PickDishCount(float gameProgress)
    {
        gameProgress = Mathf.Clamp01(gameProgress);

        float one = Mathf.Max(0, _oneDishChance.Evaluate(gameProgress));
        float two = Mathf.Max(0, _twoDishChance.Evaluate(gameProgress));
        float three = Mathf.Max(0, _threeDishChance.Evaluate(gameProgress));
        float four = Mathf.Max(0, _fourDishChance.Evaluate(gameProgress));

        float total = one + two + three + four;

        if (total <= 0)
        {
            return 1;
        }

        float randomValue = Random.Range(0, total);

        if (randomValue < one)
        {
            return 1;
        }

        randomValue -= one;

        if (randomValue < two)
        {
            return 2;
        }

        randomValue -= two;

        if (randomValue < three)
        {
            return 3;
        }

        return 4;
    }

    public CustomerOrder GenerateCustomerOrder(int difficulty)
    {
        var availableOrders = new List<CustomerOrder>();

        for (int i = 0; i < _customerOrders.Length; i++)
        {
            CustomerOrder customerOrder = _customerOrders[i];

            if ((int)customerOrder.difficulty <= difficulty && customerOrder.CheckPlayerHasIngredients())
            {
                availableOrders.Add(customerOrder);
            }
        }

        if (availableOrders.Count > 0)
        {
            int randomIndex = Random.Range(0, availableOrders.Count);
            return availableOrders[randomIndex];
        }

        return null;
    }
}