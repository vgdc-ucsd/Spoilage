using UnityEngine;

public class IngredientObject : MonoBehaviour
{
    [SerializeField] private IngredientData _data;

    public Ingredient IngredientInstance { get; private set; }

    private void Awake()
    {
        IngredientInstance = new Dough(_data); 
    }
}
