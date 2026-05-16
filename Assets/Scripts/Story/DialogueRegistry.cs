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

    /// <summary>
    /// Steepness of the resistance probability curve. At 1.0, a candidate one point below its
    /// resistance threshold carries ~37% relative weight; two points below ~14%; three ~5%.
    /// </summary>
    public const float ResistanceDecay = 1f;

    private struct SequenceLocation
    {
        public DialogueSaveFile File;
        public string Name;
    }

    private static readonly Dictionary<string, SequenceLocation> s_sequences = new();
    private static readonly List<DialogueSaveFile> s_files = new();
    private static bool s_loaded;

    /// <summary>
    /// Resolves a dialogue key to a sequence, weighted by how well each candidate matches
    /// current state. Hard attributes (time, spoilage) exclude non-matching candidates
    /// entirely; resistance attributes reduce a candidate's weight via an exponential curve
    /// rather than excluding it outright. Considers exact key plus contiguous numbered
    /// variants <c>key.0</c>, <c>key.1</c>, ...
    /// </summary>
    public static bool TryGet(string key, out DialogueSequence sequence)
    {
        EnsureLoaded();
        sequence = null;

        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        List<SequenceLocation> locs = null;
        List<float> weights = null;
        float totalWeight = 0f;

        if (s_sequences.TryGetValue(key, out SequenceLocation exact))
        {
            float w = ComputeWeight(exact);
            if (w > 0f)
            {
                (locs ??= new List<SequenceLocation>()).Add(exact);
                (weights ??= new List<float>()).Add(w);
                totalWeight += w;
            }
        }

        for (int n = 0; ; n++)
        {
            string variantKey = key + "." + n;
            if (!s_sequences.TryGetValue(variantKey, out SequenceLocation loc))
            {
                break;
            }
            float w = ComputeWeight(loc);
            if (w > 0f)
            {
                (locs ??= new List<SequenceLocation>()).Add(loc);
                (weights ??= new List<float>()).Add(w);
                totalWeight += w;
            }
        }

        if (locs == null)
        {
            return false;
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        for (int i = 0; i < locs.Count; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative)
            {
                sequence = new DialogueSequence(locs[i].File, locs[i].Name);
                return true;
            }
        }

        // Floating-point rounding fallback: take the last candidate.
        SequenceLocation last = locs[locs.Count - 1];
        sequence = new DialogueSequence(last.File, last.Name);
        return true;
    }

    /// <summary>Returns true when <see cref="TryGet"/> would succeed for the key.</summary>
    public static bool Has(string key)
    {
        return TryGet(key, out _);
    }

    /// <summary>
    /// Returns the selection weight for a candidate. Hard-filter attributes return 0 when
    /// unmatched; resistance attributes scale weight smoothly via <see cref="ResistanceCurve"/>.
    /// </summary>
    private static float ComputeWeight(SequenceLocation loc)
    {
        IReadOnlyDictionary<string, string> attrs = loc.File.GetAttributes(loc.Name);
        if (attrs == null || attrs.Count == 0)
        {
            return 1f;
        }

        PlayerData player = SaveManager.Instance != null ? SaveManager.Instance.Player : null;
        Customer customer = CustomerLineManager.Instance != null ? CustomerLineManager.Instance.CurrentCustomer : null;

        float weight = 1f;
        foreach (KeyValuePair<string, string> attr in attrs)
        {
            switch (attr.Key)
            {
                case "resistance_min":
                    if (player != null && float.TryParse(attr.Value, out float min))
                        weight *= ResistanceCurve(player.resistanceScore - min);
                    break;
                case "resistance_max":
                    if (player != null && float.TryParse(attr.Value, out float max))
                        weight *= ResistanceCurve(max - player.resistanceScore);
                    break;
                default:
                    if (!MatchesHardFilter(attr.Key, attr.Value, player, customer))
                        return 0f;
                    break;
            }
        }
        return weight;
    }

    /// <summary>
    /// Curve applied to the signed distance from a resistance threshold.
    /// Positive delta (at or above threshold) gives full weight 1.0.
    /// Negative delta (below threshold) decays exponentially toward zero.
    /// </summary>
    private static float ResistanceCurve(float delta)
    {
        if (delta >= 0f) return 1f;
        return Mathf.Exp(ResistanceDecay * delta);
    }

    private static bool MatchesHardFilter(string key, string value, PlayerData player, Customer customer)
    {
        switch (key)
        {
            case "time":
                return player != null && BucketDay(player.Day).ToString().Equals(value, System.StringComparison.OrdinalIgnoreCase);
            case "spoilage":
                return customer != null && customer.customerData != null
                    && customer.customerData.spoilage.ToString().Equals(value, System.StringComparison.OrdinalIgnoreCase);
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
