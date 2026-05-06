using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Plate : MonoBehaviour
{
    [SerializeField] private List<IngredientObject> _ingredients = new List<IngredientObject>();

    [SerializeField] private Transform _stackPoint;
    [SerializeField] private float _stackOffset = 20f;
    

    public bool AddIngredient(IngredientObject ingredient)
    {
        if (ingredient == null) return false;

        Debug.Log("AddIngredient was just called for: " + ingredient.name);

        foreach (IngredientObject existing in _ingredients)
        {
            if (existing.IngredientInstance.Data.Name == ingredient.IngredientInstance.Data.Name)
            {
                Debug.Log("Duplicate Ingredient Type detected: " + ingredient.IngredientInstance.Data.Name);
                return false;
            }
        }

        _ingredients.Add(ingredient);

        FoodGrab grab = ingredient.GetComponent<FoodGrab>();
        if (grab != null)
        {
            grab.LockToPlate();
            if (ingredient.TryGetComponent<Collider2D>(out var col)) col.enabled = false;
        }

        RectTransform rect = ingredient.GetComponent<RectTransform>();
        rect.SetParent(_stackPoint);

        Vector2 position = Vector2.zero;
        position.y += _stackOffset * (_ingredients.Count - 1);
        rect.anchoredPosition = position;

        Image img = ingredient.GetComponent<Image>();
        if (img != null)
        {
            rect.SetAsLastSibling();
        }

        return true;
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
            Debug.Log($"- {ingredient.IngredientInstance.Data.Name} ({ingredient.IngredientInstance.CurrentCookState} {ingredient.IngredientInstance.CurrentChoppedState})");
        }
    }

    [ContextMenu("Test Recipe Check")]
    public void CheckForRecipe()
    {
        // Finds the manager in the scene
        RecipeManager manager = FindAnyObjectByType<RecipeManager>();

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

