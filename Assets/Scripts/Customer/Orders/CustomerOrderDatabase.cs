using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Build;
using UnityEngine;

public class CustomerOrderDatabase : Singleton<CustomerOrderDatabase>
{
    private RecipeManager _recipeManager;

    private SaveManager _saveManager;

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

    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        _recipeManager = RecipeManager.Instance;
        _saveManager = SaveManager.Instance;
        UpdateAvailableRecipes();
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

        float randomValue = UnityEngine.Random.Range(0, total);

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

    /// <summary>
    /// Adds Recipes to the global UnlockedRecipes based on the current unlocked
    /// ingredients and appliances. Should be called every time a new recipe or
    /// appliance is unlocked.
    /// </summary>
    public void UpdateAvailableRecipes()
    {
        //wow allRecipes.allRecipes is stupid why did i do that
        foreach (Recipe recipe in _recipeManager.allRecipes.allRecipes)
        {
            if (recipe.servable)
            {
                if (UpdateAvailableRecipesHelper(recipe))
                {
                    _saveManager.Player.RecipesUnlocked.Add(recipe);
                    Debug.Log("Added " + recipe.name + " to unlocked recipes");
                }
            }
        }
    }

    /// <summary>
    /// Recursive function that goes through every required ingredient for a 
    /// recipe until it reaches the base ingredients, then checks to see if 
    /// that ingredient is unlocked.
    /// </summary>
    /// <returns></returns>
    protected bool UpdateAvailableRecipesHelper(Recipe recipe)
    {
        // BASE CASE: Base ingredient, check if unlocked if not return false
        if (recipe.requiredIngredients.Length == 0)
        {
            Predicate<string> predicate = name => name == recipe.name;
            return _saveManager.Player.IngredientsUnlocked.Exists(predicate);
            // wtf is a predicate and why do i need to use one
        }

        bool result = true;

        // APPLIANCE CHECK: If an appliance is required check to make sure its 
        // unlocked
        if (recipe.appliance != "None" && recipe.appliance != "Spoil")
        {
            Predicate<string> predicate = app => app == recipe.appliance;
            if (!_saveManager.Player.StationsUnlocked.Exists(predicate))
            {
                result = false;
            }
        }

        // INGREDIENT CHECK: Go through each ingredient until reaching base case
        foreach(RecipeRequirement ingredient in recipe.requiredIngredients)
        {
            if (!UpdateAvailableRecipesHelper(
                _recipeManager.allRecipes.allRecipes[ingredient.id]
            ))
            {
                result = false;
            }
        }
        return result;
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
            int randomIndex = UnityEngine.Random.Range(0, availableOrders.Count);
            return availableOrders[randomIndex];
        }

        return null;
    }
}