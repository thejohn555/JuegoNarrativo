using System;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeDialogueGraph : ScriptableObject
{
    public string EntryNodeID;
    public List<RuntimeDialogueNode> AllNodes = new List<RuntimeDialogueNode>();

    public RuntimeDialogueNode GetNodeByID(string id)
    {
        return AllNodes.Find(n => n.NodeID == id);
    }
}

[Serializable]
public class RuntimeDialogueNode
{
    public const float DefaultTextSpeed = 25f;
    public string NodeID;
    public string DialogueText;
    public string SpeakerName;
    public float TextSpeed = DefaultTextSpeed;
    public List<ChoiceData> Choices = new List<ChoiceData>();
    public string NextNodeID;
}

[Serializable]
public class ChoiceData
{
    public string ChoiceText;
    public string DestinationNodeID;
}
