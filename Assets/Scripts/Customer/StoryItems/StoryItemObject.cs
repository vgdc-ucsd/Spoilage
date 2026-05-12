
using UnityEngine;
using UnityEngine.UI;

public class StoryItemObject : MonoBehaviour
{
    [SerializeField] private StoryItemData _data;
    [SerializeField] private Image _image;

    public StoryItem StoryItemInstance { get; private set; }

    private void Awake()
    {
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }

        if (_image == null)
        {
            _image = GetComponentInChildren<Image>();
        }

        StoryItemInstance = new StoryItem(_data);
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }

        if (_image == null)
        {
            _image = GetComponentInChildren<Image>();
        }

        if (_image == null)
        {
            Debug.LogWarning("No Image found on " + gameObject.name);
            return;
        }

        if (StoryItemInstance == null || StoryItemInstance.Data == null)
        {
            return;
        }
    }
}