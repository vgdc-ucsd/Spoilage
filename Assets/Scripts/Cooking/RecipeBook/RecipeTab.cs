using UnityEngine;
using UnityEngine.UI;

public class RecipeTab : MonoBehaviour
{
    [SerializeField] private Image _recipeImage;
    private RecipeBook _recipeBook;
    private int _index;

    public void Init(RecipeBook recipeBook, Sprite recipeSprite, int index)
    {
        _recipeBook = recipeBook;
        _recipeImage.sprite = recipeSprite;
        _index = index;
    }

    public void Click()
    {
        AudioManager.Instance.PlaySFX("PageTurn");
        _recipeBook.SelectRecipe(_index);
    }
}
