using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace DialogueGraph.Editor
{
    [ScriptedImporter(1, DialogueGraph.AssetExtension)]
    public class DialogueGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            DialogueGraph editorGraph = GraphDatabase.LoadGraphForImporter<DialogueGraph>(ctx.assetPath);
            RuntimeDialogueGraph runtimeGraph = ScriptableObject.CreateInstance<RuntimeDialogueGraph>();
            var nodeIDMap = new Dictionary<INode, string>();

            foreach (var node in editorGraph.GetNodes())
            {
                nodeIDMap[node] = Guid.NewGuid().ToString();
            }

            var startNode = editorGraph.GetNodes().OfType<StartNode>().FirstOrDefault();
            if (startNode != null)
            {
                var entryPort = startNode.GetOutputPorts().FirstOrDefault()?.FirstConnectedPort;
                 if (entryPort != null)
                 {
                     runtimeGraph.EntryNodeID = nodeIDMap[entryPort.GetNode()];
                 }
            }
        
            foreach (var iNode in editorGraph.GetNodes())
            {
                if (iNode is StartNode || iNode is EndNode) continue;
        
                var runtimeNode = new RuntimeDialogueNode { NodeID = nodeIDMap[iNode] };
        
                switch (iNode)
                {
                    case DialogueNode dialogueNode:
                        ProccessDialogueNode(dialogueNode, runtimeNode, nodeIDMap);
                        break;
        
                    case ChoiceNode choiceNode:
                        ProccessChoiceNode(choiceNode, runtimeNode, nodeIDMap);
                        break;
        
                    default:
                        Debug.LogWarning($"El nodo de tipo {iNode.GetType()} no tiene una funcion de procesado asignada.");
                        break;
        
                }
        
                runtimeGraph.AllNodes.Add(runtimeNode);
            }
        
            ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
            ctx.SetMainObject(runtimeGraph);
        }
        
        private void ProccessDialogueNode(DialogueNode node, RuntimeDialogueNode runtimeNode,
            Dictionary<INode, string> nodeIDMap)
        {
            runtimeNode.DialogueText = GetPortValue<string>(node.GetInputPortByName(PortNames.PortDialogue));
             runtimeNode.SpeakerName = GetPortValue<string>(node.GetInputPortByName(PortNames.PortSpeaker));
             float portSpeed = GetPortValue<float>(node.GetInputPortByName("Text Speed"));
             runtimeNode.TextSpeed = portSpeed > 0 ? portSpeed : runtimeNode.TextSpeed;
            
             var nextNodePort = node.GetOutputPortByName(PortNames.PortOutput)?.FirstConnectedPort;
             if (nextNodePort != null)
             {
                 runtimeNode.NextNodeID = nodeIDMap[nextNodePort.GetNode()];
             }
        }
        
        private void ProccessChoiceNode(ChoiceNode node, RuntimeDialogueNode runtimeNode, Dictionary<INode, string> nodeIDMap)
        {
             runtimeNode.DialogueText = GetPortValue<string>(node.GetInputPortByName(PortNames.PortDialogue));
             runtimeNode.SpeakerName = GetPortValue<string>(node.GetInputPortByName(PortNames.PortSpeaker));
             float portSpeed = GetPortValue<float>(node.GetInputPortByName("Text Speed"));
             runtimeNode.TextSpeed = portSpeed > 0 ? portSpeed : runtimeNode.TextSpeed;
            
             var choiceOutputNodes = node.GetOutputPorts().Where(p => p.Name.StartsWith(PortNames.PortChoiceOutput));
            
             foreach (var outputPort in choiceOutputNodes)
             {
                 var index = outputPort.Name.Substring($"{PortNames.PortChoiceOutput}".Length);
                 var textPort = node.GetInputPortByName($"{PortNames.PortChoiceText} {index}");
            
                 var choiceData = new ChoiceData
                 {
                     ChoiceText = GetPortValue<string>(textPort),
                     DestinationNodeID = outputPort.FirstConnectedPort != null ? nodeIDMap[outputPort.FirstConnectedPort.GetNode()] : null
                 };
            
                 runtimeNode.Choices.Add(choiceData);
            }
        }
        
        private T GetPortValue<T>(IPort port)
        {
            if (port == null) return default;
        
            if (port.IsConnected)
            {
                if (port.FirstConnectedPort.GetNode() is IVariableNode variableNode)
                {
                    variableNode.Variable.TryGetDefaultValue(out T value);
                    return value;
                }
            }
        
            port.TryGetValue(out T fallbackValue);
            return fallbackValue;
         }
    }
}
