using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using Microsoft.VisualBasic;
using FMOD.Studio;
using System.Linq;

[System.Serializable]
public class SFXEntry
{
    public string id;
    public EventReference eventReference;
}

[System.Serializable]
public class MusicEntry
{
    public string id;
    public EventReference eventReference;
    [HideInInspector] public EventInstance eventInstance;
}





public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private List<SFXEntry> SFXEntries;
    private Dictionary<string, EventReference> sfxMap;
    
    /// <summary>
    /// References to FMOD events with multi-instruments, which randomly shuffle and play a list of songs
    /// </summary>
    [SerializeField] private List<MusicEntry> musicEntries;
    private Dictionary<string, MusicEntry> musicMap;
    private EventInstance currentMusicInstance;

    private FMOD.Studio.Bus masterBus;
    
    private float currentVolume = 1.0f;
    private const float MIN_VOLUME = 0.0f;
    private const float MAX_VOLUME = 1.0f;
    private const float VOLUME_STEP = 1.0f;


    public override void Awake()
    {
        base.Awake();

      

        sfxMap = new Dictionary<string, EventReference>();
        musicMap = new Dictionary<string, MusicEntry>();

        foreach (SFXEntry entry in SFXEntries)
        {
            if (!sfxMap.ContainsKey(entry.id))
            {
                sfxMap.Add(entry.id, entry.eventReference);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Duplicate SFX id found: {entry.id}");
            }
        }

        foreach (MusicEntry musicEntry in musicEntries)
        {
            musicEntry.eventInstance = RuntimeManager.CreateInstance(musicEntry.eventReference);
            musicMap.Add(musicEntry.id, musicEntry);
        }
        PlayTitleMusic();
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
        //printBusList();

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
    public void SetVolume(float volume, string busString = "bus:/")
    {
        float dB = LinearToDecibels(volume);
        Debug.Log($"Set Volume to : {dB} dB");

        FMOD.Studio.Bus bus = RuntimeManager.GetBus(busString);
        bus.setVolume(dB);
    }
    // fmod uses db, but for volume im putting 0.0 - 1.0 so its nicer to use
    // this convers to db
    public float LinearToDecibels(float linear)
    {
        if (linear <= 0.00f)
            return -80f; 
         return Mathf.Lerp(-80f, 1.0f, linear);
    }

    /// <summary>
    /// Plays one of the background music events,
    /// these include title screen, cozy, horror, shop, and radio
    /// </summary>
    /// <param name="id">Which background music entry to start playing</param>
    private void PlayMusicEntry(string id)
    {
        if (!musicMap.ContainsKey(id))
        {
            Debug.LogError($"Key {id} is not a valid music entry");
            return;
        }
        currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicMap[id].eventInstance.start();
        currentMusicInstance = musicMap[id].eventInstance;
        Debug.Log($"Playing background music with id: {id}");
    }

    

    public void PlayTitleMusic()
    {
        PlayMusicEntry("Title");
    }

    public void PlayCozyMusic()
    {
        PlayMusicEntry("Cozy");
    }

    public void PlayHorrorMusic()
    {
        PlayMusicEntry("Horror");
    }

    public void PlayShopMusic()
    {
        PlayMusicEntry("Shop");
    }

    public void PlayRadioMusic()
    {
        PlayMusicEntry("Radio");
    }

    public void PlayCreditsMusic()
    {
        PlayMusicEntry("Credits");
    }

    public void PlayLoseMusic()
    {
        PlayMusicEntry("Lose");
    }



    // https://qa.fmod.com/t/get-a-bus-list-from-a-bank/19434
    private FMOD.Studio.Bus[] myBuses = new FMOD.Studio.Bus[12];
    private string busesList;
    private string buf;
    private FMOD.Studio.Bank myBank;

    private string BusPath;
    public FMOD.RESULT busListOk;
    public FMOD.RESULT sysemIsOk;
    int busCount;
    string busPath;

    public void printBusList()
    {
        FMODUnity.RuntimeManager.StudioSystem.getBankList(out FMOD.Studio.Bank[] loadedBanks);
        foreach (FMOD.Studio.Bank bank in loadedBanks)
        {
            bank.getPath(out string path);
            busListOk = bank.getBusList(out myBuses);
            bank.getBusCount(out busCount);
            if (busCount > 0)
            {
                foreach (var bus in myBuses)
                {
                    bus.getPath(out busPath);
                    UnityEngine.Debug.Log($"{busPath}");
                }
            }
        }
    }
   
   
    
}