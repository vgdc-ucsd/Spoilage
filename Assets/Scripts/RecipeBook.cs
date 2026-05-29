using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecipeBook : MonoBehaviour
{
    [SerializeField] private GameObject recipeBookRoot;
    [SerializeField] private Image displayImage;
    [SerializeField] private List<Sprite> images;

    private int currentIndex = 0;

    private void Start()
    {
        UpdateImage();
    }

    public void NextImage()
    {
        if (images.Count == 0)
        {
            return;
        }

        currentIndex++;

        if (currentIndex >= images.Count)
        {
            currentIndex = 0;
        }

        UpdateImage();
    }

    public void PreviousImage()
    {
        if (images.Count == 0)
        {
            return;
        }

        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = images.Count - 1;
        }

        UpdateImage();
    }

    private void UpdateImage()
    {
        displayImage.sprite = images[currentIndex];
    }

    public void OpenBook()
    {
        recipeBookRoot.SetActive(true);
    }

    public void CloseBook()
    {
        recipeBookRoot.SetActive(false);
    }
}