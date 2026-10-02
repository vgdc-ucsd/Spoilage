using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
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

    public float currentVolume { get; private set; } = 0.5f;
    private const float MIN_VOLUME = 0.0f;
    private const float MAX_VOLUME = 1.0f;
    private const float VOLUME_STEP = 1.0f;


    public override void Awake()
    {
        base.Awake();

        // a duplicate singleton is being destroyed, don't set it up
        if (Instance != this) return;

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
            // event instances are created lazily in PlayMusicEntry, once FMOD has loaded its banks
            if (!musicMap.TryAdd(musicEntry.id, musicEntry))
            {
                UnityEngine.Debug.LogWarning($"Duplicate music id found: {musicEntry.id}");
            }
        }
    }

    public IEnumerator Start()
    {
        // fmod is cringe idk man but this works
        yield return new WaitUntil(() => RuntimeManager.IsInitialized && RuntimeManager.HaveAllBanksLoaded);
        masterBus = RuntimeManager.GetBus("bus:/");
        SetVolume(currentVolume);
        //printBusList();
    }

    private void OnDestroy()
    {
        if (Instance != this || musicEntries == null) return;

        foreach (MusicEntry musicEntry in musicEntries)
        {
            if (musicEntry.eventInstance.isValid())
            {
                musicEntry.eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                musicEntry.eventInstance.release();
            }
        }
    }

    public void PlaySFX(string id)
    {
        if (sfxMap.TryGetValue(id, out EventReference eventReference))
        {
            var instance = RuntimeManager.CreateInstance(eventReference);
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(Vector3.zero));
            instance.start();
            instance.release();
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
        FMOD.Studio.Bus bus = RuntimeManager.GetBus(busString);
        bus.setVolume(volume);
    }

    /// <summary>
    /// Plays one of the background music events,
    /// these include title screen, cozy, horror, shop, and radio
    /// </summary>
    /// <param name="id">Which background music entry to start playing</param>
    public void PlayMusicEntry(string id)
    {
        if (!musicMap.ContainsKey(id))
        {
            Debug.LogError($"Key {id} is not a valid music entry");
            return;
        }
        MusicEntry entry = musicMap[id];
        if (!entry.eventInstance.isValid())
        {
            entry.eventInstance = RuntimeManager.CreateInstance(entry.eventReference);
        }
        if (currentMusicInstance.isValid())
        {
            currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        entry.eventInstance.start();
        currentMusicInstance = entry.eventInstance;
        Debug.Log("Played music: " + id);
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
