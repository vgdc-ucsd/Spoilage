using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class DEBUG_PreviewIngredients : MonoBehaviour
{
#if UNITY_EDITOR
    private List<IngredientObject> _ingredientObjects = new List<IngredientObject>();
    private bool _previewingSpoilage = false;
    private bool _previewingPlating = false;
    private bool _previewingSeasoning = false;

    void Start()
    {
        _ingredientObjects = new List<IngredientObject>(GetComponentsInChildren<IngredientObject>());
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            _previewingSpoilage = !_previewingSpoilage;
            foreach (var ingredient in _ingredientObjects)
            {
                ingredient.IngredientInstance.SetSpoilagePercent(_previewingSpoilage ? 100f : 0f);
            }
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            _previewingPlating = !_previewingPlating;
            foreach (var ingredient in _ingredientObjects)
            {
                ingredient.IngredientInstance.IsPlated = !_previewingPlating;
            }
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            _previewingSeasoning = !_previewingSeasoning;
            foreach (var ingredient in _ingredientObjects)
            {
                if (_previewingSeasoning) ingredient.IngredientInstance.Season();
                else ingredient.IngredientInstance.RemoveSeasoning();
            }
        }
    }

#else
    void Awake()
    {
        Destroy(this);
    }
#endif
}
