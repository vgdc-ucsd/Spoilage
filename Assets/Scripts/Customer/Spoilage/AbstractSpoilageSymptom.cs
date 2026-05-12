using System;
using UnityEngine;

public abstract class AbstractSpoilageSymptom : ScriptableObject
{
    private static readonly Type[] symptomTypes =
    {
        typeof(BlinkingRapidly),
        typeof(Soilage),
        typeof(Sweating),
        typeof(BugSkittering),
        typeof(Gurgling),
        typeof(Tentacles),
        typeof(Ourgh),
        typeof(Ewww),
        typeof(Threat),
        typeof(OozePus),
    };

    public Customer customer;

    public SpoilageCategory category;

    public abstract void ApplySpoilage();

    public static AbstractSpoilageSymptom GenerateSymptom(Customer customer)
    {
        Type randomSymptomType = AbstractSpoilageSymptom.symptomTypes[
            UnityEngine.Random.Range(0, AbstractSpoilageSymptom.symptomTypes.Length)
        ];
        AbstractSpoilageSymptom symptom = (AbstractSpoilageSymptom)
            ScriptableObject.CreateInstance(randomSymptomType);
        symptom.customer = customer;
        // TODO: Integrate with trigger manager.
        // SpoilageTriggerManager.Instance.AddSymptom(symptom);

        return symptom;
    }

    public void DeleteSymptom()
    {
        // SpoilageTriggerManager.Instance.RemoveSymptom(this);
        Destroy(this);
    }
}
