using UnityEngine;
using System.Collections.Generic;

public class Plate : MonoBehaviour
{
    [SerializeField] private List<IngredientObject> _ingredients = new List<IngredientObject>();

    [SerializeField] private Transform _stackPoint;
    [SerializeField] private float _stackOffset = 0.1f;
    

    public void AddIngredient(IngredientObject ingredient)
    {
        Debug.Log("AddIngredient was just called for: " + ingredient.name);
        FoodGrab grab = ingredient.GetComponent<FoodGrab>();

        if (ingredient == null) return;

        if (grab != null)
        {
            grab.LockToPlate();
        }

        if (_ingredients.Contains(ingredient))
            return;

        _ingredients.Add(ingredient);

        ingredient.transform.SetParent(_stackPoint);

        Vector3 position = _stackPoint.position;
        position.y += _stackOffset * (_ingredients.Count - 1);

        SpriteRenderer sr = ingredient.GetComponent<SpriteRenderer>();
        sr.sortingOrder = _ingredients.Count;

        ingredient.transform.position = position;
    }

    public List<IngredientObject> GetIngredients()
    {
        return _ingredients;
    }

    public void PrintIngredients()
    {
        if (_ingredients.Count == 0)
        {
            Debug.Log("Plate is empty");
            return;
        }

        Debug.Log("Plate contains:");

        foreach (IngredientObject ingredient in _ingredients)
        {
            Debug.Log($"- {ingredient.IngredientInstance.Data.Name} ({ingredient.IngredientInstance.CurrentCookState})");
        }
    }

    [ContextMenu("Test Recipe Check")]
    public void CheckForRecipe()
    {
        // Finds the manager in the scene
        RecipeManager manager = FindFirstObjectByType<RecipeManager>();

        if (manager != null)
        {
            // Ask the manager if our current ingredients match anything
            string result = manager.CheckRecipe(_ingredients);
            Debug.Log("Result of Plate Check: " + result);
        }
        else
        {
            Debug.LogError("No RecipeManager found in the scene!");
        }
    }
}

