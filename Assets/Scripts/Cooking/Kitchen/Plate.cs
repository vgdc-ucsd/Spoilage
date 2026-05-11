using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Plate : MonoBehaviour
{
    [SerializeField] private IngredientObject _ingredient;

    [SerializeField] private Transform _stackPoint;
    

    public bool AddIngredient(IngredientObject ingredient)
    {
        if (ingredient == null || _ingredient != null) return false;

        Debug.Log("AddIngredient was just called for: " + ingredient.name);

        FoodGrab grab = ingredient.GetComponent<FoodGrab>();
        if (grab != null)
        {
            grab.LockToPlate();
            if (ingredient.TryGetComponent<Collider2D>(out var col)) col.enabled = false;
            Debug.Log("Ingredient " + ingredient.IngredientInstance.Data.Name + " placed, locking to plate");
            Destroy(grab);
        }

        RectTransform rect = ingredient.GetComponent<RectTransform>();
        rect.SetParent(_stackPoint);

        Vector2 position = Vector2.zero;
        rect.anchoredPosition = position;

        Image img = ingredient.GetComponent<Image>();
        if (img != null)
        {
            rect.SetAsLastSibling();
        }

        _ingredient = ingredient;
        return true;
    }

    public IngredientObject GetIngredient()
    {
        return _ingredient;
    }

    public void PrintIngredient()
    {
        if (_ingredient == null)
        {
            Debug.Log("Plate is empty");
            return;
        }

        Debug.Log("Plate contains: ");

        Debug.Log(
            $"- {_ingredient.IngredientInstance.Data.Name} " +
            $"(Spoilage: {_ingredient.IngredientInstance.SpoilagePercent:F1}%)"
        );
    }

    /*
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
    }*/
}