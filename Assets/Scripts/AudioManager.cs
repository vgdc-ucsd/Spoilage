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
   
    private FMOD.Studio.Bus masterBus;
    
    private float currentVolume = 1.0f;
    private const float MIN_VOLUME = 0.0f;
    private const float MAX_VOLUME = 1.0f;
    private const float VOLUME_STEP = 1.0f;


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
    
    public void Start()
    {
        // fmod is cringe idk man but this works
        var system = RuntimeManager.CoreSystem;
        //Debug.Log($"FMOD Is Initialized: {RuntimeManager.IsInitialized}");
        var masterBus = RuntimeManager.GetBus("bus:/");
        //Debug.Log($"Master Bus valid: {masterBus.isValid()}");    
        masterBus = RuntimeManager.GetBus("bus:/");
        SetVolume(currentVolume);


  

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
    public void IncreaseVolume(float v = 0.1f)
    {
        float newVolume = currentVolume + v;
        SetVolume(Mathf.Clamp(newVolume, MIN_VOLUME, MAX_VOLUME));
    }

    public void DecreaseVolume(float v = 0.1f)
    {
        float newVolume = currentVolume - v;
        SetVolume(Mathf.Clamp(newVolume, MIN_VOLUME, MAX_VOLUME));
    }
    public void SetVolume(float volume)
    {
        float dB = LinearToDecibels(volume);
       // Debug.Log($"Set Volume to : {dB} dB");

        masterBus.setVolume(dB);
    }
    // fmod uses db, but for volume im putting 0.0 - 1.0 so its nicer to use
    // this convers to db
    public float LinearToDecibels(float linear)
    {
        if (linear <= 0.00f)
            return -80f; 
         return Mathf.Lerp(-80f, 0.0f, linear);
    }
   
   
    
}