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
}

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private List<SFXEntry> _sfxEntries;
    private Dictionary<string, EventReference> _sfxMap;

    
    /// <summary>
    /// References to FMOD events with multi-instruments, which randomly shuffle and play a list of songs
    /// </summary>
    [SerializeField] private List<MusicEntry> _musicEntries;
    private Dictionary<string, EventReference> _musicMap;
    private EventInstance _currentMusicInstance;

    public Bus masterBus;
    public Bus musicBus;
    public Bus sfxBus;
    public Bus attenuationBus;
    private const float MIN_VOLUME = 0.0f;
    private const float MAX_VOLUME = 1.0f;
    private const float VOLUME_FACTOR = 1.5f; //sets exponent for volume scaling


    public override void Awake()
    {
        base.Awake();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/attenuation/music");
        sfxBus = RuntimeManager.GetBus("bus:/attenuation/sfx");
        attenuationBus = RuntimeManager.GetBus("bus:/attenuation");

      

        _sfxMap = new Dictionary<string, EventReference>();
        _musicMap = new Dictionary<string, EventReference>();


        foreach (SFXEntry entry in _sfxEntries)
        {
            if (!_sfxMap.ContainsKey(entry.id))
            {
                _sfxMap.Add(entry.id, entry.eventReference);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Duplicate SFX id found: {entry.id}");
            }
        }
        foreach (MusicEntry entry in _musicEntries)
        {
            if (!_musicMap.ContainsKey(entry.id))
            {
                _musicMap.Add(entry.id, entry.eventReference);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Duplicate Music id found: {entry.id}");
            }
        }
        PlayMusic("TitleMusic"); // play title screen music
    }
    
    public void Start()
    {

        // fmod is cringe idk man but this works
        SetVolume(0.5f,masterBus);
        //printBusList();
    }
    /// <summary>
    /// Only use for playing music with the radio
    /// </summary>
    /// <param name="faction">must be RESISTANCE or WARLORD</param>
    public void PlayRadioMusic(string faction)
    {
        EventInstance radioMusicInstance = RuntimeManager.CreateInstance(_musicMap["RadioMusic"]);
        if (faction == "RESISTANCE")
        {
            radioMusicInstance.setParameterByNameWithLabel("faction","RESISTANCE");
        }
        else if (faction == "WARLORD")
        {
            radioMusicInstance.setParameterByNameWithLabel("faction","WARLORD");
        }
        else
        {
            Debug.Log("Tried to play a radio track that doesn't exist!");
        }
        radioMusicInstance.start();
    }

    /// <summary>
    /// Plays a sfx one shot. No way to stop it after you start!
    /// </summary>
    /// <param name="id">Name of event you want to play</param>
    public void PlaySFX(string id)
    {
        if (_sfxMap.TryGetValue(id, out EventReference eventReference))
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
    
    public void SetVolume(float volume, Bus bus)
    {
        float adjustedVolume = AdjustVolume(volume);

        bus.setVolume(adjustedVolume);
    }
    public float GetVolume(Bus bus)
    {
        float volume;
        bus.getVolume(out volume);
        return volume;
    }
    

    /// <summary>
    /// Plays one of the background music events,
    /// these include title screen, cozy, horror, shop, and radio
    /// </summary>
    /// <param name="id">Which background music entry to start playing</param>
    public void PlayMusic(string id)
    {
        if (!_musicMap.ContainsKey(id))
        {
            Debug.LogError($"Key {id} is not a valid music entry");
            return;
        }
        _currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        EventInstance newMusicInstance = RuntimeManager.CreateInstance(_musicMap[id]);
        newMusicInstance.start();
        _currentMusicInstance = newMusicInstance;
        Debug.Log($"Playing background music with id: {id}");
    }
    private float AdjustVolume(float volume)
    {
        float clamped = Mathf.Clamp(volume,MIN_VOLUME,MAX_VOLUME);
        return Mathf.Pow(clamped,VOLUME_FACTOR);
    }
    
}