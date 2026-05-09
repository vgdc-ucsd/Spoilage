using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

[System.Serializable]
public class SFXEntry
{
    public string id;
    public EventReference eventReference;
}


public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private List<SFXEntry> SFXEntries;

    private Dictionary<string, EventReference> sfxMap;

    public override void Awake()
    {
        base.Awake();

        sfxMap = new Dictionary<string, EventReference>();

        foreach (SFXEntry entry in SFXEntries)
        {
            if (!sfxMap.ContainsKey(entry.id))
            {
                sfxMap.Add(entry.id, entry.eventReference);
            }
            else
            {
                Debug.LogWarning($"Duplicate SFX id found: {entry.id}");
            }
        }
    }

    public void PlaySFX(string id)
    {
        if (sfxMap.TryGetValue(id, out EventReference eventReference))
        {
            RuntimeManager.PlayOneShot(eventReference);
            Debug.Log("Played audio: " + id);
        }
        else
        {
            Debug.LogWarning($"SFX id not found: {id}");
        }
    }
}