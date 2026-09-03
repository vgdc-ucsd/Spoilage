using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomerOrderDatabase : Singleton<CustomerOrderDatabase>
{
    public bool Loaded { get; private set; } = false;
    
    //singleton hell
    private RecipeManager _recipeManager;
    private SaveManager _saveManager;
    private CustomerLineManager _lineManager;
    private ResourceManager _resourceManager;

    [Header("Chance curves based on game progress from 0 to 1")]
    [SerializeField]
    private AnimationCurve _oneDishChance;

    [SerializeField]
    private AnimationCurve _twoDishChance;

    [SerializeField]
    private AnimationCurve _threeDishChance;

    [SerializeField]
    private AnimationCurve _fourDishChance;

    private int orderStreak = 0;

    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        _recipeManager = RecipeManager.Instance;
        _lineManager = CustomerLineManager.Instance;
        _resourceManager = ResourceManager.Instance;

        SaveManager.OnPlayerLoad(() =>
        {
            _saveManager = SaveManager.Instance;
            UpdateAvailableRecipes();
            Loaded = true;
        });
    }

    public int PickDishCount(float gameProgress)
    {
        //divide by 30 because days are 1-30
        gameProgress = Mathf.Clamp01(gameProgress / 30);

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
        SyncUnlockedRecipes();

        foreach (Recipe recipe in _recipeManager.allRecipes.allRecipes)
        {
            if (recipe.servable)
            {
                if (CheckPlayerCanMakeRecipe(recipe) && !IsRecipeUnlocked(recipe))
                {
                    _saveManager.Player.RecipesUnlocked.Add(recipe);
                }
            }
        }
    }

    /// <summary>
    /// Recursive function that goes through every required ingredient for a 
    /// recipe until it reaches the base ingredients, then checks to see if 
    /// that ingredient is unlocked.
    /// </summary>
    public bool CheckPlayerCanMakeRecipe(Recipe recipe)
    {
        // BASE CASE: Base ingredient, check if unlocked if not return false
        if (recipe.requiredIngredients == null || recipe.requiredIngredients.Length == 0)
        {
            return _saveManager.Player.IngredientsUnlocked.Contains(recipe.name);
        }

        bool result = true;

        // APPLIANCE CHECK: If an appliance is required check to make sure its 
        // unlocked
        if (recipe.appliance != "None" && recipe.appliance != "Spoil")
        {
            if (!_saveManager.Player.StationsUnlocked.Contains(recipe.appliance))
            {
                result = false;
            }
        }

        // INGREDIENT CHECK: Go through each ingredient until reaching base case
        foreach (RecipeRequirement ingredient in recipe.requiredIngredients)
        {
            if (!CheckPlayerCanMakeRecipe(_recipeManager.allRecipes.allRecipes[ingredient.id]))
            {
                result = false;
            }
        }

        return result;
    }

    /// <summary>
    /// Generates a set of orders for the player to cook
    /// </summary>
    /// <param name="orders">A reference to the CustomerData object to modify</param>
    public void GenerateCustomerOrder(CustomerData customerData)
    {
        List<Recipe> unlockedRecipes = _saveManager.Player.RecipesUnlocked;
        int dishCount = PickDishCount(_saveManager.Player.Day);

        for (int i = 0; i < dishCount; i++)
        {
            //TODO: implement weighting

            //Default Customer
            if (customerData.spoilage != CustomerData.Spoilage.STAGE_II)
            {
                Predicate<Recipe> unspoiledRecipeCheck = x => x.spoiled == false;
                List<Recipe> unspoiledRecipes = unlockedRecipes.FindAll(unspoiledRecipeCheck);
                int rand = UnityEngine.Random.Range(0, unspoiledRecipes.Count);
                customerData.orders.Add(unspoiledRecipes[rand]);
            }

            //Spoiled Customer
            if (customerData.spoilage == CustomerData.Spoilage.STAGE_II)
            {
                Predicate<Recipe> spoiledRecipeCheck = x => x.spoiled == true;
                List<Recipe> spoiledRecipes = unlockedRecipes.FindAll(spoiledRecipeCheck);
                int rand = UnityEngine.Random.Range(0, spoiledRecipes.Count);
                customerData.orders.Add(spoiledRecipes[rand]);
            }
        }
    }

    private void SyncUnlockedRecipes()
    {
        List<Recipe> unlockedRecipes = _saveManager.Player.RecipesUnlocked;

        for (int i = 0; i < unlockedRecipes.Count; i++)
        {
            Recipe recipe = unlockedRecipes[i];

            unlockedRecipes[i] = _recipeManager.allRecipes.allRecipes[recipe.id];
        }
    }

    private bool IsRecipeUnlocked(Recipe recipe)
    {
        foreach (Recipe unlockedRecipe in _saveManager.Player.RecipesUnlocked)
        {
            if (unlockedRecipe.id == recipe.id)
            {
                return true;
            }
        }

        return false;
    }
}
