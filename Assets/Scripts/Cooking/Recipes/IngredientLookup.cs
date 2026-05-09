using UnityEngine;
using System.Collections.Generic;

public class IngredientLookup : MonoBehaviour
{
    [SerializeField] private List<IngredientData> _allPossibleIngredients;
    private static IngredientLookup s_instance;
    private Dictionary<string, IngredientData> _lookupTable;

    private void Awake()
    {
        s_instance = this;
        InitializeLookUp();
    }

    private void InitializeLookUp()
    {
        if (_lookupTable != null) return;

        _lookupTable = new Dictionary<string, IngredientData>();
        foreach (var data in _allPossibleIngredients)
        {
            if (data != null && !string.IsNullOrEmpty(data.Name))
            {
                _lookupTable[data.Name.ToLower()] = data;
            }
        }
    }

    public static IngredientData Get(string name)
    {
        if (s_instance == null)
        {
            Debug.LogError("IngredientLookup instance is missing from the scene!");
            return null;
        }

        // Ensure table is built
        s_instance.InitializeLookUp();

        if (s_instance._lookupTable.TryGetValue(name.ToLower(), out IngredientData data))
        {
            return data;
        }

        Debug.LogWarning($"IngredientLookup: No data found for '{name}'");
        return null;
    }
}