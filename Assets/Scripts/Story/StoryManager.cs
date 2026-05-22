using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coordinates story data with player progress.
/// </summary>
public class StoryManager : Singleton<StoryManager>
{
    private const int MIN_CUSTOMERS_PER_DAY = 5;
    private const float MIN_RESISTANCE = 0f;
    private const float MAX_RESISTANCE = 14f;
    private const float REFUSE_SPOILED_DELTA = 1f;
    private const float SERVE_SPOILED_DELTA = -1f;

    [SerializeField]
    private StoryDatabase _database;

    private readonly Queue<CustomerData> _customerQueue = new Queue<CustomerData>();
    private readonly List<CustomerData> _lastCustomerQueue = new List<CustomerData>();

    public StoryDatabase Database => _database;
    public IReadOnlyList<CustomerData> LastCustomerQueue => _lastCustomerQueue;
    public int QueuedSlots => _customerQueue.Count;

    /// <summary>
    /// Rolls run-level story choices when starting a new run.
    /// </summary>
    public void InitRun(bool forceReroll = false)
    {
        PlayerData player = Player;

        if (forceReroll || player.Day == 0)
        {
            player.Day = 1;
        }

        if (forceReroll || string.IsNullOrEmpty(player.activeSetId))
        {
            player.activeSetId = RollSetId();
            player.activeAdditionalId = RollExtraId();
        }

        Save();
    }

    /// <summary>
    /// Resets per-day story counters and builds today's guaranteed customer queue.
    /// </summary>
    public void BeginDay()
    {
        PlayerData player = Player;

        player.semiKeyRefusedToday = 0;
        player.semiImportantDishesFailedToday = 0;

        BuildCustomerQueue();
        Save();
    }

    /// <summary>
    /// Evaluates story conditions that schedule content for the following day.
    /// </summary>
    public void EndDay()
    {
        EvalReactionaries();
        Save();
    }

    public void AdvanceDay()
    {
        Player.Day++;
        Save();
    }

    public DayEntry GetCurrentDayEntry()
    {
        return GetDayEntry(Player.Day);
    }

    public CustomerData GetBeginInteraction()
    {
        DayEntry entry = GetCurrentDayEntry();
        return entry == null ? null : GetInteraction(entry.beginInteraction);
    }

    public CustomerData GetEndInteraction()
    {
        DayEntry entry = GetCurrentDayEntry();
        return entry == null ? null : GetInteraction(entry.endInteraction);
    }

    /// <summary>
    /// Rebuilds the queue used to guarantee story customers during cooking.
    /// </summary>
    public void BuildCustomerQueue()
    {
        _customerQueue.Clear();
        _lastCustomerQueue.Clear();

        PlayerData player = Player;

        List<List<CustomerData>> blocks = new List<List<CustomerData>>();
        DayEntry entry = GetDayEntry(player.Day);

        if (entry != null)
        {
            AddSingles(blocks, entry.semiKeys);
            AddPairs(blocks, entry.semiKeyPairs);
        }

        AddReactionaries(blocks, player);

        int storySlots = CountSlots(blocks);
        int randomSlots = Mathf.Max(0, MIN_CUSTOMERS_PER_DAY - storySlots);
        for (int i = 0; i < randomSlots; i++)
        {
            blocks.Add(new List<CustomerData> { null });
        }

        Shuffle(blocks);

        for (int blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
        {
            List<CustomerData> block = blocks[blockIndex];
            for (int i = 0; i < block.Count; i++)
            {
                CustomerData data = block[i];
                _customerQueue.Enqueue(data);
                _lastCustomerQueue.Add(data);
            }
        }

        player.pendingReactionaryIds.Clear();
    }

    public bool TryDequeueCustomer(out CustomerData customerData)
    {
        customerData = null;
        if (_customerQueue.Count == 0)
        {
            return false;
        }

        customerData = _customerQueue.Dequeue();
        return true;
    }

    public void OnCustomerRefused(CustomerData customerData)
    {
        PlayerData player = Player;
        string id = customerData.id;
        if (!string.IsNullOrEmpty(id))
        {
            AddUnique(player.refusedCharacterIds, id);
        }

        if (customerData.tier == CustomerData.Tier.SemiKey)
        {
            player.semiKeyRefusedToday++;
            player.semiKeyRefusedLifetime++;
        }

        if (customerData.spoilage != CustomerData.Spoilage.UNSPOILED)
        {
            AddResistance(player, REFUSE_SPOILED_DELTA);
        }

        Save();
    }

    public void OnCustomerServed(CustomerData customerData, bool success)
    {
        PlayerData player = Player;
        string id = customerData.id;
        if (success && !string.IsNullOrEmpty(id))
        {
            AddUnique(player.servedCharacterIds, id);
        }

        if (!success && customerData.tier == CustomerData.Tier.SemiKey)
        {
            player.semiImportantDishesFailedToday++;
            player.semiImportantDishesFailedLifetime++;
        }

        if (success && customerData.spoilage != CustomerData.Spoilage.UNSPOILED)
        {
            AddResistance(player, SERVE_SPOILED_DELTA);
        }

        Save();
    }

    public void EvalReactionaries()
    {
        PlayerData player = Player;
        List<ReactionaryRule> rules = _database.reactionary.rules;
        for (int i = 0; i < rules.Count; i++)
        {
            ReactionaryRule rule = rules[i];
            if (!ConditionNode.EvaluateOrTrue(rule.condition, player)) continue;
            if (IsRefused(rule.spawnsCharacter)) continue;

            string id = rule.spawnsCharacter.id;
            if (!string.IsNullOrEmpty(id))
            {
                AddUnique(player.pendingReactionaryIds, id);
            }
        }
    }

    public RadioEntry GetRadioBroadcast()
    {
        PlayerData player = Player;

        List<RadioEntry> entries = _database.radio.entries;
        List<RadioEntry> filler = new List<RadioEntry>();
        for (int i = 0; i < entries.Count; i++)
        {
            RadioEntry entry = entries[i];
            if (!ConditionNode.EvaluateOrTrue(entry.condition, player)) continue;

            if (entry.isFiller)
            {
                filler.Add(entry);
            }
            else
            {
                return entry;
            }
        }

        if (filler.Count == 0)
        {
            return null;
        }

        return filler[Random.Range(0, filler.Count)];
    }

    public string GetRadioDialogueKey()
    {
        RadioEntry entry = GetRadioBroadcast();
        return entry == null ? null : entry.dialogueKey;
    }

    public CustomerData FindCharacter(string id)
    {
        CustomerData found = FindInRoster(id);
        if (found != null) return found;

        found = FindInTimeline(id);
        if (found != null) return found;

        return FindInReactionaryRules(id);
    }

    private PlayerData Player => SaveManager.Instance.Player;

    private string RollSetId()
    {
        if (_database.roster.semiKeySets.Count == 0)
        {
            return null;
        }

        List<SemiKeySet> sets = _database.roster.semiKeySets;
        SemiKeySet picked = sets[Random.Range(0, sets.Count)];
        return picked.id;
    }

    private string RollExtraId()
    {
        if (_database.roster.additionalCharacters.Count == 0)
        {
            return null;
        }

        List<AdditionalCharacterEntry> entries = _database.roster.additionalCharacters;
        float totalWeight = 0f;
        for (int i = 0; i < entries.Count; i++)
        {
            AdditionalCharacterEntry entry = entries[i];
            totalWeight += entry.weight;
        }

        if (totalWeight <= 0f)
        {
            return null;
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        for (int i = 0; i < entries.Count; i++)
        {
            AdditionalCharacterEntry entry = entries[i];

            cumulative += entry.weight;
            if (roll <= cumulative)
            {
                return entry.character == null ? null : entry.character.id;
            }
        }

        AdditionalCharacterEntry last = entries[entries.Count - 1];
        return last.character == null ? null : last.character.id;
    }

    private DayEntry GetDayEntry(int day)
    {
        List<DayEntry> days = _database.timeline.days;
        for (int i = 0; i < days.Count; i++)
        {
            DayEntry entry = days[i];
            if (entry.day == day)
            {
                return entry;
            }
        }

        return null;
    }

    private CustomerData GetInteraction(CustomerData customerData)
    {
        if (customerData == null) return null;
        if (IsRefused(customerData)) return null;
        return IsRunEligible(customerData) ? customerData : null;
    }

    private void AddSingles(List<List<CustomerData>> blocks, List<CustomerData> customers)
    {
        for (int i = 0; i < customers.Count; i++)
        {
            CustomerData customer = customers[i];
            if (!IsRunEligible(customer)) continue;
            if (IsRefused(customer)) continue;

            blocks.Add(new List<CustomerData> { customer });
        }
    }

    private void AddPairs(List<List<CustomerData>> blocks, List<SemiKeyPair> pairs)
    {
        for (int i = 0; i < pairs.Count; i++)
        {
            SemiKeyPair pair = pairs[i];
            if (!IsRunEligible(pair.first) || !IsRunEligible(pair.second)) continue;
            if (IsRefused(pair.first) || IsRefused(pair.second)) continue;

            blocks.Add(new List<CustomerData> { pair.first, pair.second });
        }
    }

    private void AddReactionaries(List<List<CustomerData>> blocks, PlayerData player)
    {
        for (int i = 0; i < player.pendingReactionaryIds.Count; i++)
        {
            CustomerData customer = FindCharacter(player.pendingReactionaryIds[i]);
            if (customer == null)
            {
                Debug.LogError($"No reactionary character found with id '{player.pendingReactionaryIds[i]}'.");
                continue;
            }
            if (IsRefused(customer)) continue;

            blocks.Add(new List<CustomerData> { customer });
        }
    }

    private bool IsRunEligible(CustomerData customerData)
    {
        if (customerData.tier != CustomerData.Tier.SemiKey) return true;

        string id = customerData.id;
        if (string.IsNullOrEmpty(id)) return false;
        if (id == Player.activeAdditionalId) return true;

        SemiKeySet activeSet = FindSet(Player.activeSetId);
        if (activeSet == null) return _database.roster.semiKeySets.Count == 0;

        for (int i = 0; i < activeSet.members.Count; i++)
        {
            CustomerData member = activeSet.members[i];
            if (member.id == id)
            {
                return true;
            }
        }

        return false;
    }

    private SemiKeySet FindSet(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        List<SemiKeySet> sets = _database.roster.semiKeySets;
        for (int i = 0; i < sets.Count; i++)
        {
            SemiKeySet set = sets[i];
            if (set.id == id)
            {
                return set;
            }
        }

        return null;
    }

    private bool IsRefused(CustomerData customerData)
    {
        if (string.IsNullOrEmpty(customerData.id))
        {
            return false;
        }

        return Player.refusedCharacterIds.Contains(customerData.id);
    }

    private CustomerData FindInRoster(string id)
    {
        CustomerData found = FindInList(_database.roster.keyCharacters, id);
        if (found != null) return found;

        List<SemiKeySet> sets = _database.roster.semiKeySets;
        for (int i = 0; i < sets.Count; i++)
        {
            SemiKeySet set = sets[i];
            found = FindInList(set.members, id);
            if (found != null) return found;
        }

        List<AdditionalCharacterEntry> additional = _database.roster.additionalCharacters;
        for (int i = 0; i < additional.Count; i++)
        {
            AdditionalCharacterEntry entry = additional[i];
            if (entry.character != null && entry.character.id == id)
            {
                return entry.character;
            }
        }

        return null;
    }

    private CustomerData FindInTimeline(string id)
    {
        List<DayEntry> days = _database.timeline.days;
        for (int i = 0; i < days.Count; i++)
        {
            DayEntry entry = days[i];

            if (MatchesId(entry.beginInteraction, id)) return entry.beginInteraction;
            if (MatchesId(entry.endInteraction, id)) return entry.endInteraction;

            CustomerData found = FindInList(entry.semiKeys, id);
            if (found != null) return found;

            for (int pairIndex = 0; pairIndex < entry.semiKeyPairs.Count; pairIndex++)
            {
                SemiKeyPair pair = entry.semiKeyPairs[pairIndex];
                if (MatchesId(pair.first, id)) return pair.first;
                if (MatchesId(pair.second, id)) return pair.second;
            }
        }

        return null;
    }

    private CustomerData FindInReactionaryRules(string id)
    {
        List<ReactionaryRule> rules = _database.reactionary.rules;
        for (int i = 0; i < rules.Count; i++)
        {
            ReactionaryRule rule = rules[i];
            if (MatchesId(rule.spawnsCharacter, id))
            {
                return rule.spawnsCharacter;
            }
        }

        return null;
    }

    private static CustomerData FindInList(List<CustomerData> customers, string id)
    {
        for (int i = 0; i < customers.Count; i++)
        {
            CustomerData customer = customers[i];
            if (MatchesId(customer, id))
            {
                return customer;
            }
        }

        return null;
    }

    private static bool MatchesId(CustomerData customerData, string id)
    {
        return customerData != null && customerData.id == id;
    }

    private static int CountSlots(List<List<CustomerData>> blocks)
    {
        int count = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            count += blocks[i].Count;
        }

        return count;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[swapIndex];
            list[swapIndex] = temp;
        }
    }

    private static void AddUnique(List<string> values, string value)
    {
        if (values.Contains(value))
        {
            return;
        }

        values.Add(value);
    }

    private void AddResistance(PlayerData player, float delta)
    {
        player.resistanceScore = Mathf.Clamp(
            player.resistanceScore + delta,
            MIN_RESISTANCE,
            MAX_RESISTANCE);
    }

    private void Save()
    {
        SaveManager.Instance.SaveGame();
    }
}
