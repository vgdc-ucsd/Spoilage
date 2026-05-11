using System.Collections.Generic;
using TextboxControl;
using UnityEngine;

public enum TimeDivision
{
    Early,
    Mid,
    Late
}

public enum DialoguePoint
{
    Intro,
    Refuse,
    ServeSuccess,
    ServeFail
}

/// <summary>
/// Resolved dialogue sequence from an entry.
/// </summary>
public sealed class DialogueSequence
{
    private readonly DialogueSaveFile _file;
    private readonly string _name;

    internal DialogueSequence(DialogueSaveFile file, string name)
    {
        _file = file;
        _name = name;
    }

    public int Count => _file.CountBoxes(_name);
    public string GetBox(int index) => _file.GetBox(_name, index);
}

/// <summary>
/// Global lookup from dialogue key to a <see cref="DialogueSequence"/>. Takes into account game state and
/// .N* prefixes when resolving.
/// </summary>
public static class DialogueRegistry
{
    /// <summary>Folder under <c>Resources/</c> scanned for TextAssets.</summary>
    public const string ResourcesFolder = "Dialogue";

    private struct SequenceLocation
    {
        public DialogueSaveFile File;
        public string Name;
    }

    private static readonly Dictionary<string, SequenceLocation> s_sequences = new();
    private static readonly List<DialogueSaveFile> s_files = new();
    private static bool s_loaded;

    /// <summary>
    /// Resolves a dialogue key to a sequence whose attribute predicates accept current state.
    /// Considers exact key plus contiguous numbered variants <c>key.0</c>, <c>key.1</c>, ...
    /// and picks randomly from those that match.
    /// </summary>
    public static bool TryGet(string key, out DialogueSequence sequence)
    {
        EnsureLoaded();
        sequence = null;

        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        List<SequenceLocation> matches = null;

        if (s_sequences.TryGetValue(key, out SequenceLocation exact) && Accepts(exact))
        {
            (matches ??= new List<SequenceLocation>()).Add(exact);
        }

        for (int n = 0; ; n++)
        {
            string variantKey = key + "." + n;
            if (!s_sequences.TryGetValue(variantKey, out SequenceLocation loc))
            {
                break;
            }
            if (Accepts(loc))
            {
                (matches ??= new List<SequenceLocation>()).Add(loc);
            }
        }

        if (matches == null)
        {
            return false;
        }

        SequenceLocation pick = matches[Random.Range(0, matches.Count)];
        sequence = new DialogueSequence(pick.File, pick.Name);
        return true;
    }

    /// <summary>Returns true when <see cref="TryGet"/> would succeed for the key.</summary>
    public static bool Has(string key)
    {
        return TryGet(key, out _);
    }

    private static bool Accepts(SequenceLocation loc)
    {
        IReadOnlyDictionary<string, string> attrs = loc.File.GetAttributes(loc.Name);
        if (attrs == null || attrs.Count == 0)
        {
            return true;
        }

        PlayerData player = SaveManager.Instance != null ? SaveManager.Instance.Player : null;
        Customer customer = CustomerLineManager.Instance != null ? CustomerLineManager.Instance.CurrentCustomer : null;

        foreach (KeyValuePair<string, string> attr in attrs)
        {
            if (!MatchesState(attr.Key, attr.Value, player, customer))
            {
                return false;
            }
        }
        return true;
    }

    private static bool MatchesState(string key, string value, PlayerData player, Customer customer)
    {
        switch (key)
        {
            case "time":
                return player != null && BucketDay(player.Day).ToString().Equals(value, System.StringComparison.OrdinalIgnoreCase);
            case "spoilage":
                return customer != null && customer.customerData != null
                    && customer.customerData.spoilage.ToString().Equals(value, System.StringComparison.OrdinalIgnoreCase);
            case "resistance_min":
                return player != null && float.TryParse(value, out float min) && player.resistanceScore >= min;
            case "resistance_max":
                return player != null && float.TryParse(value, out float max) && player.resistanceScore <= max;
            default:
                return true;
        }
    }

    private static TimeDivision BucketDay(int day)
    {
        if (day <= 10) return TimeDivision.Early;
        if (day <= 20) return TimeDivision.Mid;
        return TimeDivision.Late;
    }

    private static void EnsureLoaded()
    {
        if (s_loaded)
        {
            return;
        }
        s_loaded = true;

        TextAsset[] assets = Resources.LoadAll<TextAsset>(ResourcesFolder);
        for (int i = 0; i < assets.Length; i++)
        {
            TextAsset asset = assets[i];
            DialogueSaveFile parsed;
            try
            {
                parsed = DialogueSaveFile.Parse(asset.text);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DialogueRegistry] Failed to parse '{asset.name}': {e.Message}");
                continue;
            }

            s_files.Add(parsed);

            foreach (string name in parsed.EnumerateSequenceNames())
            {
                if (s_sequences.ContainsKey(name))
                {
                    Debug.LogError($"[DialogueRegistry] Duplicate sequence '{name}' encountered in '{asset.name}'. Earlier definition kept.");
                    continue;
                }
                s_sequences[name] = new SequenceLocation { File = parsed, Name = name };
            }
        }
    }
}
