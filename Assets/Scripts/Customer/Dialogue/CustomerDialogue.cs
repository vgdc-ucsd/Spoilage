using System;
using System.Collections.Generic;

[Serializable]
public class DialogueItemData
{
    public string ID;
    public string Stage;
    public List<string> Success;
    public List<string> Fail;
}

[Serializable]
public class CustomerDialogue
{
    public List<string> Intro;
    public List<string> Success;
    public List<string> Fail;
    public List<string> Reject;
    public List<DialogueItemData> Items;
}
