using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Interactions
{
    [SerializeField] private List<CustomerData> _beginInteraction;
    [SerializeField] private List<CustomerData> _middleInteraction;
    [SerializeField] private List<CustomerData> _endInteraction;

    public List<CustomerData> BeginInteraction => _beginInteraction;
    public List<CustomerData> MiddleInteraction => _middleInteraction;
    public List<CustomerData> EndInteraction => _endInteraction;
}

public class InteractionSet
{
    public InteractionSet(List<Interactions> interactions)
    {
        BeginInteractions = interactions
            .Select(interaction => interaction.BeginInteraction)
            .Where(begin => begin != null)
            .ToList();

        MiddleInteractions = interactions
            .Select(interaction => interaction.MiddleInteraction)
            .Where(middle => middle != null)
            .ToList();

        EndInteractions = interactions
            .Select(interaction => interaction.EndInteraction)
            .Where(end => end != null)
            .ToList();
    }
   
    public List<List<CustomerData>> BeginInteractions { get; private set; }
    public List<List<CustomerData>> MiddleInteractions { get; private set; }
    public List<List<CustomerData>> EndInteractions { get; private set; }
}

[CreateAssetMenu(fileName = "InteractionsNode", menuName = "Progression/InteractionsNode")]
public class InteractionsNode : GraphNode<Interactions> { }