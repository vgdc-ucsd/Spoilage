using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngredientRequirement
{
    public string ingredientName;
}

[System.Serializable]
public class Recipe
{
    public string dishName;
    public string station;
    public bool requiresAllSpoiled;
    public List<IngredientRequirement> ingredients;
}

[System.Serializable]
public class RecipeList
{
    public List<Recipe> recipes;
}

public class RecipeManager : MonoBehaviour
{
    public TextAsset recipeJsonFile; // Drag your JSON file here in the Inspector!
    public RecipeList allRecipes;

    void Awake()
    {
        LoadRecipes();
    }

    void LoadRecipes()
    {
        if (recipeJsonFile != null)
        {
            // THIS is what fills the "brain" of the manager
            allRecipes = JsonUtility.FromJson<RecipeList>(recipeJsonFile.text);
            Debug.Log("Recipe Manager: Loaded " + allRecipes.recipes.Count + " recipes from JSON.");
        }
        else
        {
            Debug.LogError("Recipe Manager: No JSON file assigned in the Inspector!");
        }
    }

    public string CheckRecipe(List<IngredientObject> ingredients, string station = "")
    {
        Debug.Log($"Recipe Manager: Starting check for {ingredients.Count} items.");

        if (allRecipes == null || allRecipes.recipes == null)
        {
            return "JSON Error";
        }

        foreach (Recipe recipe in allRecipes.recipes)
        {
            if (!string.IsNullOrEmpty(station) && recipe.station != station)
            continue;
            
            if (IsMatch(recipe, ingredients))
            {
                return recipe.dishName;
            }
        }

        return "Slop";
    }

    private bool IsMatch(Recipe recipe, List<IngredientObject> plateIngredients)
    {
        if (recipe == null || plateIngredients == null) return false;
        if (recipe.ingredients.Count != plateIngredients.Count) return false;

        if (recipe.requiresAllSpoiled)
        {
            foreach (IngredientObject food in plateIngredients)
            {
                if (food == null || food.IngredientInstance == null) return false;

                if (!food.IngredientInstance.IsSpoiled)
                {
                    return false;
                }
            }
        }

        List<string> remainingRequirements = new List<string>();

        foreach (IngredientRequirement req in recipe.ingredients)
        {
            remainingRequirements.Add(req.ingredientName.Trim().ToLower());
        }

        foreach (IngredientObject food in plateIngredients)
        {
            if (food == null || food.IngredientInstance == null) continue;

            string plateName = food.IngredientInstance.Data.Name.Trim().ToLower();

            if (!remainingRequirements.Remove(plateName))
            {
                return false;
            }
        }

        return remainingRequirements.Count == 0;
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