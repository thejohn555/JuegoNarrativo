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
    private DialogueData currentDialogueData;
    private RuntimeDialogueNode currentNode;

    private void OnEnable()
    {
        EventBus.Subscribe<OnDialogueStartedEvent>(DisplayFirstNode);
        if (BlueprintsEventManager.Instance != null)// Nuevo hecho por Nacho
            BlueprintsEventManager.Instance.StartNpcDialogue += StartNpcDialogue;// Nuevo hecho por Nacho
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<OnDialogueStartedEvent>(DisplayFirstNode);
        if (BlueprintsEventManager.Instance != null)// Nuevo hecho por Nacho
            BlueprintsEventManager.Instance.StartNpcDialogue -= StartNpcDialogue;// Nuevo hecho por Nacho
    }
    
    private void StartNpcDialogue(GameObject npcObject)// Nuevo hecho por Nacho
    {
        if (!npcObject.TryGetComponent<Interactives.NPC>(out var npc))
        {
            Debug.LogWarning($"'{npcObject.name}' no tiene componente NPC");
            return;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        StartDialogue(npc.CurrentGraph, npc.BuildDialogueData(player));
    }
    public void StartDialogue(RuntimeDialogueGraph graph, DialogueData data)
    {
        if (graph == null) return;

        Debug.Log($"[StartDialogue] NPC ID recibido: '{data.npcID}'");
        
        currentGraph = graph;
        currentDialogueData = data;
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
        Debug.Log($"[EndDialogue] NPC ID guardado: '{currentDialogueData.npcID}'");
        
        string finishedNpcId = currentDialogueData.npcID ?? string.Empty;
        RuntimeDialogueGraph finishedGraph = currentGraph;
        
        currentDialogueData = default;
        currentGraph = null;
        currentNode = null;

        EventBus.Publish(new OnDialogueEndedEvent
        {
            NpcID = finishedNpcId,
            FinishedGraph = finishedGraph
        });
        
    }
}
