using System;
using UnityEngine;

public abstract class AbstractSpoilageSymptom : ScriptableObject
{
    public static readonly Type[] symptomTypes =
    {
        typeof(BlinkingRapidly),
        //typeof(Soilage),
        //typeof(Sweating),
        typeof(BugSkittering),
        // typeof(Gurgling),
        // typeof(Tentacles),
        // typeof(Ourgh),
        // typeof(Ewww),
        // typeof(Threat),
        typeof(OozePus),
    };

    public GameObject customer;

    public SpoilageCategory category;

    public abstract void ApplySpoilage();

        public void DeleteSymptom()
    {
        // SpoilageTriggerManager.Instance.RemoveSymptom(this);
        Destroy(this);
    }
}
