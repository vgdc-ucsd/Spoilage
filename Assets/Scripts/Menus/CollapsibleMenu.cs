using UnityEngine;

public class CollapsibleMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuContent;
    private bool isExpanded = true;
    [SerializeField] private RectTransform toggleButton;
    [SerializeField] private float openMenuXPosition = 0f;
    [SerializeField] private float closedMenuXPosition = 0f;

    public void ToggleMenu()
    {
        isExpanded = !isExpanded;
        menuContent.SetActive(isExpanded);
        Debug.Log("ToggleMenu called, isExpanded: " + isExpanded);

        Vector2 pos = toggleButton.anchoredPosition;
        pos.x = isExpanded ? openMenuXPosition : closedMenuXPosition;
        toggleButton.anchoredPosition = pos;
    }

    private void Start()
    {
        menuContent.SetActive(isExpanded);
    }
}