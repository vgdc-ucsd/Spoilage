using UnityEngine;
using UnityEngine.UI;

public class StarRatingSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite halfStar;
    [SerializeField] private Sprite fullStar;
    [SerializeField] private Image[] starImages;
    private ResourceManager resourceManager;

    void Start()
    {
        starImages = GameObject.FindGameObjectWithTag("StarRating").GetComponentsInChildren<Image>();
        resourceManager = FindAnyObjectByType<ResourceManager>();
        float rating = resourceManager ?  Mathf.Round(ResourceManager.Instance.Reputation * 10f / 14f) : 0;
        UpdateStarRating(rating / 2.0f);
    }

    public void UpdateStarRating(float rating)
    {
        int fullStarCount = Mathf.FloorToInt(rating);
        bool hasHalfStar = (rating - fullStarCount) >= 0.5f;
        for(int i = 0;i < starImages.Length; i++)
        {
            if(i < fullStarCount)
            {
                starImages[i].sprite = fullStar;
            }
            else if(i == fullStarCount && hasHalfStar)
            {
                starImages[i].sprite = halfStar;
            }
            else
            {
                starImages[i].sprite = emptyStar;
            }
        }
    }
}
