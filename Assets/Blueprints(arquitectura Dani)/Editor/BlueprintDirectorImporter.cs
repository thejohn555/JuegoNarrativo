using System;
using System.Collections.Generic;
using System.Linq;
using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor
{
    [ScriptedImporter(1,BlueprintDirectorGraph.AssetExtension)]
    public class BlueprintDirectorImporter :ScriptedImporter
    {
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            Debug.Log($"[Importer] Importando: {ctx.assetPath}");
            var graph = GraphDatabase.LoadGraphForImporter<BlueprintDirectorGraph>(ctx.assetPath);

            if (graph == null)
            {
                Debug.Log($"Failed to load Blueprint director graph asset: {ctx.assetPath}");
                return;
            }
            //Seguramente haya que quitar esto xq no solo vamos a empezar desde el start
            var startNodeModel = graph.GetNodes().OfType<StartNode>().FirstOrDefault();
            if (startNodeModel == null)
            {
                Debug.LogError("[Importer] El grafo no tiene StartNode");
                return;
            }
            var runtimeAsset = ScriptableObject.CreateInstance<BlueprintRuntimeGraph>();
            var nodeMap = new Dictionary<INode, int>();
            
            //First pass: Create all runtime nodes (without conections)
            CreateRuntimeNodes(startNodeModel,runtimeAsset, nodeMap);
            //Second pass: Set up connections using the index;
            SetUpConnections(startNodeModel,runtimeAsset, nodeMap);
            
            Debug.Log($"[Importer] Terminado: {runtimeAsset.RuntimeNodes.Count} nodos");
            
            ctx.AddObjectToAsset("RunTimeAsset", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
        }

        void CreateRuntimeNodes(INode startNode, BlueprintRuntimeGraph runtimeGraph, Dictionary<INode, int> nodeMap)
        {
            var nodesToProcess = new Queue<INode>();
            nodesToProcess.Enqueue(startNode);
            while (nodesToProcess.Count > 0)
            {
                var currentNode = nodesToProcess.Dequeue();
                if (nodeMap.ContainsKey(currentNode)) continue;
                var runtimeNodes = TranslateNodeModelToRuntimeNodes(currentNode);

                foreach (var runtimeNode in runtimeNodes)
                {
                    nodeMap[currentNode] = runtimeGraph.RuntimeNodes.Count;
                    runtimeGraph.RuntimeNodes.Add(runtimeNode);
                }
                
                //Queue up all conected nodes
                for (int i = 0; i < currentNode.OutputPortCount; i++)
                {
                    var port = currentNode.GetOutputPort(i);
                    if (port.IsConnected)
                    {
                        nodesToProcess.Enqueue(port.FirstConnectedPort.GetNode());
                    }
                }
            }
        }

        void SetUpConnections(INode startNode, BlueprintRuntimeGraph runtimeGraph, Dictionary<INode, int> nodeMap)
        {
            foreach (var kvp in nodeMap)
            {
                var editorNode = kvp.Key;
                var runtimeIndex = kvp.Value;
                var runtimeNode = runtimeGraph.RuntimeNodes[runtimeIndex];

                for (int i = 0; i < editorNode.OutputPortCount; i++)
                {
                    var port =editorNode.GetOutputPort(i);

                    if (port.IsConnected && nodeMap.TryGetValue(port.FirstConnectedPort.GetNode(), out int nextIndex))
                    {
                        runtimeNode.NextNodeIndex.Add(nextIndex);
                    }
                }
            }
        }
        static List<RuntimeNode> TranslateNodeModelToRuntimeNodes(INode nodeModel)
        {
            var returnNodes = new List<RuntimeNode>();
            switch (nodeModel) //Hya que ir añadiendo todos los tipos de nodos que creemos
            {
                case StartNode startNode:
                    returnNodes.Add(new StartRuntimeNode());
                    break;
                case ConditionNode conditionNode: //Este hay que borrarlo es solo para probar
                    var conditionPort = conditionNode.GetInputPortByName("Transition");
                    if (conditionPort == null)
                    {
                        Debug.LogError("[Importer] ConditionNode no tiene un puerto con ese nombre");
                        returnNodes.Add(new ConditionRuntimeNode());
                        break;
                    }
                    var condition = GetInputPortValue<int>(conditionPort);
                    returnNodes.Add(new ConditionRuntimeNode());
                    break;
                default:
                    throw new ArgumentException($"Unsupported node type: {nodeModel.GetType()}");
            }

            return returnNodes;
        }
        static T GetInputPortValue<T>(IPort port)
        {
            if (port == null) return default;
            T value = default;
            if (port.IsConnected)
            {
                switch (port.FirstConnectedPort.GetNode())
                {
                    case IVariableNode variableNode :
                        variableNode.Variable.TryGetDefaultValue<T>(out value);
                        return value;
                    case IConstantNode constantNode :
                        constantNode.TryGetValue<T>(out value);
                        return value;
                    default:
                        break;
                }
            }
            else
            {
                port.TryGetValue<T>(out value);
            }
            
            return value;
        }
    }
    
}