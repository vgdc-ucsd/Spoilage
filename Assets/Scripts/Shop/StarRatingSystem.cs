using UnityEngine;
using UnityEngine.UI;

public class StarRatingSystem : MonoBehaviour
{
    [SerializeField] private Sprite _emptyStar;
    [SerializeField] private Sprite _halfStar;
    [SerializeField] private Sprite _fullStar;
    [SerializeField] private Image[] _starImages;
    private ResourceManager _resourceManager;

    void Start()
    {
        _resourceManager = FindAnyObjectByType<ResourceManager>();
        float rating = _resourceManager ?  Mathf.Round(ResourceManager.Instance.Reputation * 10f / 14f) : 0;
        UpdateStarRating(rating / 2.0f);
    }

    public void UpdateStarRating(float rating)
    {
        int fullStarCount = Mathf.FloorToInt(rating);
        bool hasHalfStar = (rating - fullStarCount) >= 0.5f;
        for(int i = 0;i < _starImages.Length; i++)
        {
            if(i < fullStarCount)
            {
                _starImages[i].sprite = _fullStar;
            }
            else if(i == fullStarCount && hasHalfStar)
            {
                _starImages[i].sprite = _halfStar;
            }
            else
            {
                _starImages[i].sprite = _emptyStar;
            }
        }
    }
}
