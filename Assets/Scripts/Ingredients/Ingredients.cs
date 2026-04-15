using UnityEngine;

public abstract class IngredientData : MonoBehaviour
{
    [SerializeField]
    private string _ingredientName;

    [SerializeField]
    private bool _isSpoiled;

    [SerializeField]
    private bool _isGrillable;

    public string IngredientName => _ingredientName;
    public bool IsSpoiled => _isSpoiled;
    public bool IsGrillable => _isGrillable;
}

[CreateAssetMenu(fileName = "NewDough", menuName = "Ingredients/Dough")]
public class Dough : IngredientData
{
}

[CreateAssetMenu(fileName = "NewMeat", menuName = "Ingredients/Meat")]
public class Meat : IngredientData
{
}

[CreateAssetMenu(fileName = "NewRoots", menuName = "Ingredients/Roots")]
public class Roots : IngredientData
{
}

[CreateAssetMenu(fileName = "NewCheese", menuName = "Ingredients/Cheese")]
public class Cheese : IngredientData
{
}
