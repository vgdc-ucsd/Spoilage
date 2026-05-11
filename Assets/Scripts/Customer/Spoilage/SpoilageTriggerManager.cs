using System;
using System.Collections.Generic;

public class SpoilageTriggerManager : Singleton<SpoilageTriggerManager>
{
    private Dictionary<SpoilageCategory, Action> _triggerMap =
        new Dictionary<SpoilageCategory, Action>();

    public void AddSymptom(AbstractSpoilageSymptom symptom)
    {
        SpoilageCategory category = symptom.category;
        if (!_triggerMap.ContainsKey(category))
        {
            _triggerMap.Add(category, new Action(() => { }));
        }

        _triggerMap[category] += symptom.ApplySpoilage;
    }

    public void RemoveSymptom(AbstractSpoilageSymptom symptom)
    {
        _triggerMap[symptom.category] -= symptom.ApplySpoilage;
    }

    public void Invoke(SpoilageCategory category)
    {
        _triggerMap[category]();
    }
}
