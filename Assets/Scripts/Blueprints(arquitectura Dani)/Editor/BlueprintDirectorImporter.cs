using System;
using System.Collections.Generic;
using System.Linq;
using Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor;
using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime;
using NUnit.Framework;
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
            //var startNodeModel = graph.GetNodes().OfType<StartNode>();
            //Guardamos en una lista todos los nodos de start
            List<INode> roots = graph.GetNodes().OfType<IStartNode>().Cast<INode>().ToList();
            if (roots.Count == 0)
            {
                Debug.LogError("[Importer] El grafo no tiene StartNode");
                return;
            }
            var runtimeAsset = ScriptableObject.CreateInstance<BlueprintRuntimeGraph>();
            List<Dictionary<INode, int>> nodeMaps = new();
            
            //First pass: Create all runtime nodes (without conections)
            CreateRuntimeNodes(roots,runtimeAsset, nodeMaps);
            //Second pass: Set up connections using the index;
            SetUpConnections(roots,runtimeAsset, nodeMaps);
            
            Debug.Log($"[Importer] Terminado: {runtimeAsset.RuntimeNodes.Count} nodos");
            
            ctx.AddObjectToAsset("RunTimeAsset", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
        }

        void CreateRuntimeNodes(List<INode> roots, BlueprintRuntimeGraph runtimeGraph, List<Dictionary<INode, int>> nodeMaps)
        {
            var nodesToProcess = new Queue<INode>();
            foreach (var node in roots)
            {
                RuntimeNodeGroup NodePack = new RuntimeNodeGroup();
                Dictionary<INode, int> nodeMap = new();
                nodesToProcess.Enqueue(node);
                while (nodesToProcess.Count > 0)
                {
                    var currentNode = nodesToProcess.Dequeue();
                    if (nodeMap.ContainsKey(currentNode)) continue;
                    var runtimeNodes = TranslateNodeModelToRuntimeNodes(currentNode);

                    foreach (var runtimeNode in runtimeNodes)
                    {
                        nodeMap[currentNode] = NodePack.Nodes.Count;
                        NodePack.Nodes.Add(runtimeNode);
                        Debug.Log($"[Importer] Nodo añadido {runtimeNode}");
                        //runtimeGraph.RuntimeNodes.Add(runtimeNode);
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
                runtimeGraph.RuntimeNodes.Add(NodePack);
                nodeMaps.Add(nodeMap);
            }
        }

        void SetUpConnections(List<INode> roots, BlueprintRuntimeGraph runtimeGraph, List<Dictionary<INode, int>> nodeMaps)
        {
            for(int k=0;k<roots.Count;k++)
            {
                var nodeGroup = runtimeGraph.RuntimeNodes[k];
                var map = nodeMaps[k];
                foreach (var kvp in map)
                {
                    var editorNode = kvp.Key;
                    var runtimeIndex = kvp.Value;
                    var runtimeNode = nodeGroup.Nodes[runtimeIndex];

                    for (int i = 0; i < editorNode.OutputPortCount; i++)
                    {
                        var port = editorNode.GetOutputPort(i);

                        if (port.IsConnected &&
                            map.TryGetValue(port.FirstConnectedPort.GetNode(), out int nextIndex))
                        {
                            runtimeNode.NextNodeIndex.Add(nextIndex);
                            //node.NextNodeIndex.Add(nextIndex);
                        }
                    }
                }
            }
        }
        static List<RuntimeNode> TranslateNodeModelToRuntimeNodes(INode nodeModel)
        {
            List<RuntimeNode> retornedNodes = new List<RuntimeNode>();
            NodeTranslator.AnalizeNodes(nodeModel, retornedNodes);
            return retornedNodes;

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