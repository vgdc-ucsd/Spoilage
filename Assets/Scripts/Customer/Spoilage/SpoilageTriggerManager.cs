using System;
using System.Collections.Generic;

public class SpoilageTriggerManager : Singleton<SpoilageTriggerManager>
{
    private Dictionary<SpoilageCategory, Action> _triggerMap =
        new Dictionary<SpoilageCategory, Action>();

    public override void Awake()
    {
        for (int i = 0; i < Enum.GetNames(typeof(SpoilageCategory)).Length; i++)
        {
            _triggerMap.Add((SpoilageCategory)i, () => { });
        }
        base.Awake();
    }

    public void AddSymptom(AbstractSpoilageSymptom symptom)
    {
        _triggerMap[symptom.category] += symptom.ApplySpoilage;
    }

    public void RemoveSymptom(AbstractSpoilageSymptom symptom)
    {
        _triggerMap[symptom.category] -= symptom.ApplySpoilage;
    }

    public void Invoke(SpoilageCategory category)
    {
        _triggerMap[category].Invoke();
    }
}
