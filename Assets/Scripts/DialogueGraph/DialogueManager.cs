using Assets.Scripts.Camera;
using Assets.Scripts.EventBuses;
using System;
using System.Collections.Generic;
using Blueprints_arquitectura_Dani_.Runtime;
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

    private void Start()
    {
        if (BlueprintsEventManager.Instance == null) return;

        BlueprintsEventManager.Instance.StartDialogue = (graph, data) =>
        {
            var dialogueData = data is DialogueData d
                ? d
                : new DialogueData(new Dictionary<string, SpeakerData>(), null, Vector3.zero);

            StartDialogue(graph as RuntimeDialogueGraph, dialogueData);
        };
    }

    public void StartDialogue(RuntimeDialogueGraph graph, DialogueData data)
    {
        if (graph == null) return;

        currentGraph = graph;
        EventBus.Publish(new OnDialogueDataEvent(data));
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
