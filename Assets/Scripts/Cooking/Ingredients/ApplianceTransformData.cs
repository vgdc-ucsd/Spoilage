using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IngredientTransform
{
    public IngredientData input;
    public IngredientData output;

    public bool canOvercook;
    public IngredientData overcookedOutput;
}

[CreateAssetMenu(menuName = "Cooking/Appliance Transform Data")]
public class ApplianceTransformData : ScriptableObject
{
    [SerializeField] private List<IngredientTransform> _transforms = new();

    public bool TryGetOutput(IngredientData input, out IngredientData output)
    {
        foreach (IngredientTransform transform in _transforms)
        {
            if (transform.input == input)
            {
                output = transform.output;
                return true;
            }
        }

        output = null;
        return false;
    }
}