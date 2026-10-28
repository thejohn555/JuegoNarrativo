using Assets.Scripts.Camera;
using Assets.Scripts.EventBuses;
using System;
using EventBuses;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private EventBusSO EventBus;
    private RuntimeDialogueGraph currentGraph;
    private RuntimeDialogueNode currentNode;

    private void OnEnable()
    {
        EventBus.Subscribe<OnDialogueStartedEvent>(DisplayFirstNode);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<OnDialogueStartedEvent>(DisplayFirstNode);
    }

    public void StartDialogue(RuntimeDialogueGraph graph, DialogueCameraData data)
    {
        if (graph == null) return;

        currentGraph = graph;
        EventBus.Publish(new OnDialogueCameraDataEvent(data));
    }

    private void DisplayFirstNode(OnDialogueStartedEvent ev)
    {
        DisplayNode(currentGraph.EntryNodeID);
    }

    public void DisplayNode(string nodeID)
    {
        currentNode = currentGraph.GetNodeByID(nodeID);

        if (string.IsNullOrEmpty(nodeID) || currentNode == null)
        {
            EndDialogue();
            return;
        }

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
        EventBus.Publish(new OnDialogueEndedEvent());
    }
}
