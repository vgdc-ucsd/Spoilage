using System.Collections.Generic;
using System.Linq;

public class InteractionSet
{
    public InteractionSet(List<InteractionsNode> nodes, int day)
    {
        BeginInteractions = nodes
            .Where(node => node != null && node.Day == day)
            .Select(node => node.Data.BeginInteraction)
            .Where(begin => begin != null)
            .ToList();

        MiddleInteractions = nodes
            .Where(node => node != null && node.Day == day)
            .Select(node => node.Data.MiddleInteraction)
            .Where(middle => middle != null)
            .ToList();

        EndInteractions = nodes
            .Where(node => node != null && node.Day == day)
            .Select(node => node.Data.EndInteraction)
            .Where(end => end != null)
            .ToList();
    }
   
    public List<List<Conversation>> BeginInteractions { get; private set; }
    public List<List<Conversation>> MiddleInteractions { get; private set; }
    public List<List<Conversation>> EndInteractions { get; private set; }
}
