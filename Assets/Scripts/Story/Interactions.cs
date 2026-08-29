using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Conversation
{
    [SerializeField] private CustomerData _customer;
    [SerializeField] private TextAsset _conversationJson;

    public CustomerData Customer => _customer;
    public TextAsset ConversationJson => _conversationJson; 
}

[Serializable]
public class Interactions
{
    [SerializeField] private List<Conversation> _beginInteraction;
    [SerializeField] private List<Conversation> _middleInteraction;
    [SerializeField] private List<Conversation> _endInteraction;

    public List<Conversation> BeginInteraction => _beginInteraction;
    public List<Conversation> MiddleInteraction => _middleInteraction;
    public List<Conversation> EndInteraction => _endInteraction;
}
