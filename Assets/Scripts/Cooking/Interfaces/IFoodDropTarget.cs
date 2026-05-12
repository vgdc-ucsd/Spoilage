public interface IFoodDropTarget
{
    bool CanAcceptFood(FoodGrab food);
    void AcceptFood(IngredientObject food);
    void ClearFood(IngredientObject food);
}