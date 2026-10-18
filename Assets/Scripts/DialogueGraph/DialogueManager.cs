using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public event Action<RuntimeDialogueNode> OnNodeDisplayed;
    public event Action OnDialogueEnded;

    private RuntimeDialogueGraph currentGraph;
    private RuntimeDialogueNode currentNode;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void StartDialogue(RuntimeDialogueGraph graph)
    {
        if (graph == null) return;

        currentGraph = graph;
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

        OnNodeDisplayed?.Invoke(currentNode);
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
        OnDialogueEnded?.Invoke();
    }
}
