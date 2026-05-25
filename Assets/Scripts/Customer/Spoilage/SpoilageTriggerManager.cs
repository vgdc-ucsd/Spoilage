using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoilageTriggerManager : Singleton<SpoilageTriggerManager>
{
    private const string DialoguePrefix = "spoilage.symptom";
    private const float DialogueAdvanceDelay = 0.35f;

    private readonly Dictionary<SpoilageCategory, Action> _triggerMap = CreateTriggerMap();

    public void AddSymptom(AbstractSpoilageSymptom symptom)
    {
        _triggerMap[symptom.category] += symptom.ApplySpoilageOnce;
    }

    public void RemoveSymptom(AbstractSpoilageSymptom symptom)
    {
        _triggerMap[symptom.category] -= symptom.ApplySpoilageOnce;
    }

    public void Invoke(SpoilageCategory category)
    {
        _triggerMap[category].Invoke();
    }

    public static void Trigger(SpoilageCategory category)
    {
        Instance.Invoke(category);
    }

    public static void TriggerIf(SpoilageCategory category, bool condition)
    {
        if (condition)
        {
            Trigger(category);
        }
    }

    public static bool IsUnspoiledFood(IngredientObject food)
    {
        return !food.IngredientInstance.IsSpoiled;
    }

    public static bool ContainsUnspoiledFood(IEnumerable<IngredientObject> foods)
    {
        foreach (IngredientObject food in foods)
        {
            if (IsUnspoiledFood(food))
            {
                return true;
            }
        }

        return false;
    }

    private static Dictionary<SpoilageCategory, Action> CreateTriggerMap()
    {
        Dictionary<SpoilageCategory, Action> triggerMap = new Dictionary<SpoilageCategory, Action>();

        foreach (SpoilageCategory category in Enum.GetValues(typeof(SpoilageCategory)))
        {
            triggerMap.Add(category, () => { });
        }

        return triggerMap;
    }
}
