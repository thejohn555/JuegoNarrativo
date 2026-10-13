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
            }
        }

        private void ProccessDialogueNode(DialogueNode node, RuntimeDialogueNode runtimeNode,
            Dictionary<INode, string> nodeIDMap)
        {
            //runtimeNode.DialogueText = GetPortValue<string>(node.GetInputPortByName(node.PortDialogue));
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
