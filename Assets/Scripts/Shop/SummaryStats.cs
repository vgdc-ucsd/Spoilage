using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SummaryStats : MonoBehaviour
{

    public List<string> incomeNames = new List<string>();
    public List<int> incomeAmounts = new List<int>();

    public List<string> costNames = new List<string>();
    public List<int> costAmounts = new List<int>();

    // Money
    public TextMeshProUGUI moneyText;
    public float money;

    public int customersServed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMoneyText();
    }

    void UpdateMoneyText()
    {
        float updatedMoney = (float)Math.Round((double)SaveManager.Instance.Player.Wealth, 2);
        moneyText.text = "Money: $" + updatedMoney;
    }

    public void AddIncome(string source, int amount)
    {
        incomeNames.Add(source);
        incomeAmounts.Add(amount);
    }

    public void AddCost(string source, int amount)
    {
        costNames.Add(source);
        costAmounts.Add(amount);
    }

    public int GetTotalIncome()
    {
        int total = 0;
        foreach (int amount in incomeAmounts)
            total += amount;

        return total;
    }

    public int GetTotalCosts()
    {
        int total = 0;
        foreach (int amount in costAmounts)
            total += amount;

        return total;
    }

    public int GetNetMoney()
    {
        return GetTotalIncome() - GetTotalCosts();
    }

}
