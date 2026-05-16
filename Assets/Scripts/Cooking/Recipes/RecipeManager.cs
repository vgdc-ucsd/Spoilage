using System;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

[System.Serializable]
public class RecipeRequirement
{
    public string name;
    public int id;
}

[System.Serializable]
public class Recipe
{
    public int id;
    public string name;
    public int complexity;
    public int reward;
    public string appliance;
    public bool servable;
    public bool spoiled;
    public RecipeRequirement[] requiredIngredients;

    //for stage 2 spoiled recipes
    public bool requiresAllSpoiled;
}

[System.Serializable]
public class RecipeList
{
    public Recipe[] allRecipes;
}

public class RecipeManager : Singleton<RecipeManager>
{
    public TextAsset recipeJsonFile; // Drag your JSON file here in the Inspector!
    public RecipeList allRecipes;

    public override void Awake()
    {
        base.Awake();
        LoadRecipes();
    }

    /// <summary>
    /// Pulls all recipes from json doc into allRecipes 
    /// </summary>
    void LoadRecipes()
    {
        if (recipeJsonFile != null)
        {
            // THIS is what fills the "brain" of the manager
            allRecipes = JsonUtility.FromJson<RecipeList>(recipeJsonFile.text);
            Debug.Log("MANAGER: Loaded " + allRecipes.allRecipes.Length + " recipes from JSON.");
        }
        else
        {
            // Debug.LogError("Recipe Manager: No JSON file assigned in the Inspector!");
            
        }
    }

    public string CheckRecipe(List<IngredientObject> ingredients, string station = "")
    {
        Debug.Log($"Recipe Manager: Starting check for {ingredients.Count} items.");

        if (allRecipes == null || allRecipes.allRecipes == null)
        {
            return "JSON Error";
        }

        foreach (Recipe recipe in allRecipes.allRecipes)
        {
            if (!string.IsNullOrEmpty(station) && recipe.appliance != station)
            continue;
            
            if (IsMatch(recipe, ingredients))
            {
                return recipe.name;
            }
        }

        return "Slop";
    }

    private bool IsMatch(Recipe recipe, List<IngredientObject> plateIngredients)
    {
        if (recipe == null || plateIngredients == null) return false;
        if (recipe.requiredIngredients.Length != plateIngredients.Count) return false;

        List<string> remainingRequirements = new List<string>();

        foreach (RecipeRequirement req in recipe.requiredIngredients)
        {
            remainingRequirements.Add(req.name.Trim().ToLower());
        }

        foreach (IngredientObject food in plateIngredients)
        {
            if (food == null || food.IngredientInstance == null) continue;

            string baseName = food.IngredientInstance.Data.Name.Trim().ToLower();

            if (remainingRequirements.Contains(baseName))
            {
                //stage 2 logic
                if (recipe.spoiled && !food.IngredientInstance.IsSpoiled)
                {
                    return false; 
                }

                // Remove it from the list and keep checking other ingredients
                remainingRequirements.Remove(baseName);
            }
            else
            {
                return false;
            }
        }

        return remainingRequirements.Count == 0;
    }

    public float CalculateTotalQuality(List<IngredientObject> plateIngredients)
    {
        float totalQualityPercentage = 0;
        foreach (IngredientObject food in plateIngredients)
        {
            totalQualityPercentage += food.IngredientInstance.Data.QualityPercent;
        }
        return totalQualityPercentage;
    }
    public float GetAverageSpoilage(List<IngredientObject> ingredients)
    {
        if (ingredients == null || ingredients.Count == 0)
        {
            return 0f;
        }

        float totalSpoilage = 0f;

        foreach (IngredientObject ingredient in ingredients)
        {
            if (ingredient == null || ingredient.IngredientInstance == null) continue;

            totalSpoilage += ingredient.IngredientInstance.SpoilagePercent;
        }

        return totalSpoilage / ingredients.Count;
    }
}