using System.Collections.Generic;
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
    public int rarityWeight;
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
    public const string JsonErrorResult = "JSON Error";
    public const string SlopResult = "Slop";

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
            Debug.LogError("Recipe Manager: No JSON file assigned in the Inspector!");
        }
    }

    public IngredientData LookupResult(List<Food> ingredients, Station station)
    {
        List<Recipe> matchingRecipes = new List<Recipe>();

        foreach (Recipe recipe in allRecipes.allRecipes)
        {
            if (station == null)
            {
                if (recipe.appliance != "Kitchen Tile") continue;
            }
            else if (station.Data.StationName != recipe.appliance) continue;
            
            if (IsMatch(recipe, ingredients))
            {
                matchingRecipes.Add(recipe);
            }
        }

        if (matchingRecipes.Count == 0)
        {
            return IngredientLookup.Get(SlopResult);
        }
        else if (matchingRecipes.Count == 1)
        {
            return IngredientLookup.Get(matchingRecipes[0].name);
        }
        else
        {
            return IngredientLookup.Get(DisambiguateRecipe(matchingRecipes, ingredients));
        }
    }

    private string DisambiguateRecipe(List<Recipe> matchingRecipes, List<Food> ingredients)
    {
        // there can only be two possible recipes with the same ingredients; one spoiled, one not spoiled
        // if both exist, first check if all ingredients are spoiled, if so return the spoiled recipe
        // if not return the unspoiled recipe

        Recipe spoiledRecipe = matchingRecipes.Find(r => r.spoiled);
        Recipe unspoiledRecipe = matchingRecipes.Find(r => !r.spoiled);

        foreach (Food ingredient in ingredients)
        {
            if (ingredient == null) continue;

            if (!ingredient.IsSpoiled)
            {
                return unspoiledRecipe != null ? unspoiledRecipe.name : SlopResult;
            }
        }

        // If all ingredients are spoiled, return the spoiled recipe
        return spoiledRecipe != null ? spoiledRecipe.name : SlopResult;
    }

    public static bool IsSuccessfulRecipeResult(string resultName)
    {
        return !string.IsNullOrEmpty(resultName)
            && resultName != SlopResult
            && resultName != JsonErrorResult;
    }

    private bool IsMatch(Recipe recipe, List<Food> plateIngredients)
    {
        if (recipe == null || plateIngredients == null) return false;
        if (recipe.requiredIngredients.Length != plateIngredients.Count) return false;

        List<string> remainingRequirements = new List<string>();

        foreach (RecipeRequirement req in recipe.requiredIngredients)
        {
            remainingRequirements.Add(req.name.Trim().ToLower());
        }

        foreach (Food food in plateIngredients)
        {
            if (food == null) continue;

            string baseName = food.Data.Name.Trim().ToLower();

            if (remainingRequirements.Contains(baseName))
            {
                //stage 2 logic
                if (recipe.spoiled && !food.IsSpoiled)
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
}