using Assets.Scripts.EventBuses;
using System;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    private RuntimeDialogueGraph currentGraph;
    private RuntimeDialogueNode currentNode;

    public void StartDialogue(RuntimeDialogueGraph graph)
    {
        if (graph == null) return;

        currentGraph = graph;
        EventBus.Instance.Publish(new OnDialogueStartedEvent());
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

        EventBus.Instance.Publish(new OnNodeDisplayedEvent(currentNode));
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
        EventBus.Instance.Publish(new OnDialogueEndedEvent());
    }
}
