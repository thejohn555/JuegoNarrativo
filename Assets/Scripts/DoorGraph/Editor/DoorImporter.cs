using System.Collections.Generic;
using System.Linq;
using DoorGraph.Editor.Node;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace DoorGraph.Editor
{
    [ScriptedImporter(1,DoorGraph.AssetExtension)]
    public class DoorImporter:ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var graph = GraphDatabase.LoadGraphForImporter<DoorGraph>(ctx.assetPath);
            if (graph == null) return;

            var asset = ScriptableObject.CreateInstance<DoorGraphAsset>();
            CreateConexions(graph, asset);

            ctx.AddObjectToAsset("main", asset);
            ctx.SetMainObject(asset);
            
            
        }

        void CreateConexions(DoorGraph graph,DoorGraphAsset asset)
        {
            //Primera pasada assignar un indice por nodo
            var doorNodes = graph.GetNodes().OfType<DoorBaseNode>().ToList();
            Dictionary<DoorBaseNode, int> indexOfNodes = new();
            for (int i = 0; i < doorNodes.Count; i++)
            {
                indexOfNodes[doorNodes[i]] = i;
                asset.nodes.Add(new DoorNodeData { id = doorNodes[i].Key });
            }
            //Segunda pasada leemos la salida
            for (int i = 0; i < doorNodes.Count; i++)
            {
                foreach (IPort port in doorNodes[i].GetOutputPorts())
                {
                    var targetNode = port.FirstConnectedPort?.GetNode() as DoorBaseNode;

                    int target = -1;
                    if (targetNode != null && indexOfNodes.TryGetValue(targetNode, out int idx))
                    {
                        target = idx;
                    }

                    asset.nodes[i].outputs.Add(target);
                }
            }
        }
    }
}