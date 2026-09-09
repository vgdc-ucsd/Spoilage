using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Radio : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tmp;
    [SerializeField] private List<TextAsset> _warlordFiller;
    [SerializeField] private List<TextAsset> _resistanceFiller;
    [SerializeField] private List<TextAsset> _generalFiller;

    private const float TYPEWRITER_SPEED = 0.05f;
    private const float PAUSE_DURATION = 1f;

    private class RadioLines
    {
        public List<string> lines;
    }

    void Start()
    {
        SaveManager.OnPlayerLoad(PlayRadio);
    }

    private void PlayRadio()
    {
        RadioNode radio = ProgressionManager.Instance.RadioNode;
        if (radio.Day != SaveManager.Instance.Player.Day) PlayFiller();
        else ShowText(radio.Data);
    }

    private void PlayFiller()
    {
        float resistance = SaveManager.Instance.Player.resistanceScore;
        float warlordChance;
        float resistanceChance;

        if (resistance > 10)
        {
            warlordChance = 0.0f;
            resistanceChance = 0.7f;
        }
        else if (resistance < 4)
        {
            warlordChance = 0.7f;
            resistanceChance = 0.0f;
        }
        else
        {
            warlordChance = 0.2f;
            resistanceChance = 0.2f;
        }

        List<TextAsset> radioEntries;
        float random = Random.Range(0.0f, 1.0f);
        if (random < warlordChance) radioEntries = _warlordFiller;
        else if (random < warlordChance + resistanceChance) radioEntries = _resistanceFiller;
        else radioEntries = _generalFiller;

        int randomIndex = Random.Range(0, radioEntries.Count);
        TextAsset textFile = radioEntries[randomIndex]; 
        ShowText(textFile);
    }

    private void ShowText(TextAsset textFile)
    {
        List<string> lines = JsonUtility.FromJson<RadioLines>(textFile.text).lines;
        StartCoroutine(Typewriter(lines));
    }

    private IEnumerator Typewriter(List<string> lines)
    {
        foreach (string line in lines)
        {
            _tmp.text = line;
            _tmp.maxVisibleCharacters = 0;
            
            for (int i = 0; i < _tmp.textInfo.characterCount; i++)
            {
                _tmp.maxVisibleCharacters++;
                yield return new WaitForSeconds(TYPEWRITER_SPEED);                
            }

            yield return new WaitForSeconds(PAUSE_DURATION);
        }

        _tmp.text = "";
    }
}
