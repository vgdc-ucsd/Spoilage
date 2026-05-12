#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;

// This is a debug class for testing purposes only. All of this logic should be transferred to the logic
// that handles when a customer first arrives and orders. If the customer has an item to give the player,
// then they should spawn the item on the serving area tile for the player to pick up as shown here.
public class DEBUG_SpawnStoryItem : MonoBehaviour
{
    [SerializeField] private StoryItemData _storyItemData;
    [SerializeField] private GameObject _storyItemPrefab;
    private GameObject _servingAreaTile;
    private KitchenTile _servingAreaKitchenTile;

    void Start()
    {
        _servingAreaTile = GameObject.FindWithTag("Serving Area Tile");
        _servingAreaKitchenTile = _servingAreaTile.GetComponent<KitchenTile>();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GameObject newStoryItem = Instantiate(_storyItemPrefab, _servingAreaTile.transform.position, Quaternion.identity);
            _servingAreaKitchenTile.PlaceObject(newStoryItem);
            newStoryItem.GetComponent<StoryItemGrab>().currentTile = _servingAreaKitchenTile;
            newStoryItem.GetComponent<StoryItemGrab>().Initialize();
        }
    }
}
#endif
