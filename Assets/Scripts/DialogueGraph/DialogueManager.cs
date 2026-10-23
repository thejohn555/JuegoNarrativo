using Assets.Scripts.EventBuses;
using System;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private EventBusSO EventBus;
    private RuntimeDialogueGraph currentGraph;
    private RuntimeDialogueNode currentNode;

    public void StartDialogue(RuntimeDialogueGraph graph)
    {
        if (graph == null) return;

        currentGraph = graph;
        //Felipe
        EventBus.Publish(new OnDialogueStartedEvent());
        DisplayNode(graph.EntryNodeID);
    }

    public void DisplayNode(string nodeID)
    {
        currentNode = currentGraph.GetNodeByID(nodeID);

        if (string.IsNullOrEmpty(nodeID) || currentNode == null)
        {
            EndDialogue();
            return;
        }

        //Felipe
        EventBus.Publish(new OnNodeDisplayedEvent(currentNode));
    }

    public void AdvanceToNextNode()
    {
        if (currentNode == null) return;

        if (!string.IsNullOrEmpty(currentNode.NextNodeID))
        {
            DisplayNode(currentNode.NextNodeID);
        }
        else
        { 
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        currentGraph = null;
        currentNode = null;
        //Felipe
        EventBus.Publish(new OnDialogueEndedEvent());
    }
}
