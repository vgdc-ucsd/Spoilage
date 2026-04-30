using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldButton : MonoBehaviour
{
    [Header("Ascend Settings")]
    public float ascendSpeed = 800f;
    public float ascendDuration = 1.5f;

    private RectTransform _rectTransform;
    private Button _button;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();

        // 1. Initial State: Appliances can move, Food is stuck
        LockLayout.IsLocked = false;
        FoodGrab.CanMoveFood = false;

        // 2. Automatically link the button click
        if (_button != null)
        {
            _button.onClick.AddListener(Trigger);
        }
    }

    public void Trigger()
    {
        // 3. Flip logic: Lock appliances, unlock food
        LockLayout.IsLocked = true;
        FoodGrab.CanMoveFood = true;

        // 4. Disable interaction and start moving
        if (_button != null) _button.interactable = false;
        StartCoroutine(AscendAndHide());

        Debug.Log("Day Started: Layout Locked, Food Unlocked!");
    }

    private IEnumerator AscendAndHide()
    {
        float elapsed = 0f;
        while (elapsed < ascendDuration)
        {
            // Move UI element up
            _rectTransform.anchoredPosition += new Vector2(0, ascendSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
