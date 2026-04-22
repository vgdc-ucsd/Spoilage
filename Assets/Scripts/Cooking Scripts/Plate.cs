using UnityEngine;
using System.Collections.Generic;

public class Plate : MonoBehaviour
{
    private List<IngredientObject> _ingredients = new List<IngredientObject>();

    [SerializeField] private Transform _stackPoint;
    [SerializeField] private float _stackOffset = 0.1f;

    public void AddIngredient(IngredientObject ingredient)
    {
        FoodGrab grab = ingredient.GetComponent<FoodGrab>();
        if (grab != null)
        {
            grab.LockToPlate();
        }

        if (ingredient == null) return;

        if (_ingredients.Contains(ingredient))
            return;

        _ingredients.Add(ingredient);

        ingredient.transform.SetParent(_stackPoint);

        Vector3 position = _stackPoint.position;
        position.y += _stackOffset * (_ingredients.Count - 1);

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
            Debug.Log($"- {ingredient.IngredientInstance.Data.Name} ({ingredient.IngredientInstance.CurrentState})");
        }
    }
}
