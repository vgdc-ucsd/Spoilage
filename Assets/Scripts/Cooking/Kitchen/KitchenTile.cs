using UnityEngine;
using System.Collections.Generic;

public class KitchenTile : MonoBehaviour
{
    [SerializeField] private GameObject _counterPrefab;

    [Header("Tile Inventory")]
    public List<GameObject> objectsOnTile = new List<GameObject>();

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (LockLayout.Instance != null)
            LockLayout.Instance.RegisterTile(this);
        else
            Debug.LogWarning($"KitchenTile '{name}': LockLayout instance not found!");
    }

    public bool CanPlaceObject(string type, GameObject movingObj = null)
    {
        // Always allow re-snapping the same object
        if (movingObj != null && objectsOnTile.Contains(movingObj)) return true;

        if (SetupManager.Instance == null)
        {
            Debug.LogWarning("SetupManager instance not found!");
            return false;
        }

        if (SetupManager.Instance.CurrentPhase == GamePhase.Setup)
        {
            // Setup: appliances only, one per tile
            if (type != "Appliance") return false;

            foreach (var obj in objectsOnTile)
                if (obj != null && obj.TryGetComponent(out ObjectGrab _)) return false;

            return true;
        }

        if (SetupManager.Instance.CurrentPhase == GamePhase.Cooking)
        {
            // Cooking: food only, one per tile
            if (type != "Food") return false;

            foreach (var obj in objectsOnTile)
                if (obj != null && obj.GetComponent<IngredientObject>() != null) return false;

            return true;
        }

        return false;
    }

    public void PlaceObject(GameObject obj)
    {
        if (!objectsOnTile.Contains(obj))
            objectsOnTile.Add(obj);

        // Snap to tile using RectTransform (all objects are UI)
        RectTransform objRect = obj.GetComponent<RectTransform>();
        if (objRect != null && _rectTransform != null)
        {
            objRect.SetParent(_rectTransform, false);
            objRect.anchoredPosition = Vector2.zero;
        }
        else
        {
            Debug.LogWarning($"KitchenTile '{name}': Could not snap '{obj.name}' — missing RectTransform.");
        }
    }

    public void RemoveObject(GameObject obj)
    {
        objectsOnTile.Remove(obj);
    }

    public GameObject GetTopObject()
    {
        // Clean out any destroyed objects first
        objectsOnTile.RemoveAll(o => o == null);
        return objectsOnTile.Count > 0 ? objectsOnTile[objectsOnTile.Count - 1] : null;
    }

    public bool HasAppliance()
    {
        foreach (var obj in objectsOnTile)
            if (obj != null && obj.TryGetComponent(out ObjectGrab _)) return true;
        return false;
    }

    public bool HasFood()
    {
        foreach (var obj in objectsOnTile)
            if (obj != null && obj.GetComponent<IngredientObject>() != null) return true;
        return false;
    }

    public void SetCountertopIfEmpty()
    {
        if (_counterPrefab == null)
        {
            Debug.LogWarning($"KitchenTile '{name}': Counter prefab not assigned.");
            return;
        }

        if (objectsOnTile.Count == 0)
        {
            GameObject counter = Instantiate(_counterPrefab, transform);
            RectTransform counterRect = counter.GetComponent<RectTransform>();
            if (counterRect != null) counterRect.anchoredPosition = Vector2.zero;
            PlaceObject(counter);
        }
    }
}