
using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using Blueprints.Scripts;
using Blueprints.Scripts.BaseClases;
using UnityEngine;

[Serializable]
[UseWithGraph(typeof(PuzzleGraphTest))]
public class TestNode : NodeData
{
    public override NodeType Type => NodeType.NodeTest1;

    public override List<PortData> Ports =>
        new List<PortData>()
        {
            new PortData(0, PortType.Int, PortDirection.Input),
            new PortData(0, PortType.Int, PortDirection.Output),
        };

    protected  override void OnDefinePorts(Node.IPortDefinitionContext context)
    {
        foreach (var port in Ports)
        {
            if (port.Direction == PortDirection.Input)
            { 
                context.AddInputPort<int>("In " + port.Id).Build();
            }
            else
            {
                context.AddOutputPort<int>("Out " + port.Id).Build();
            }
        }
    }
    
    public override void CollectConexions()
    {
        foreach (var port in Ports)
        {
            if (!port.IsConected(this))
            {
                Debug.Log($"Sin conexión: {Type} ({Id}) salida {port.Id}");
                continue;
            }
            List<IPort> conectedPorts = new List<IPort>();
            var outputs = GetOutputPort(port.Id);
            outputs.GetConnectedPorts(conectedPorts);
            foreach (var output in conectedPorts)
            {
                var inode = output.GetNode();
                var outputNode = SearchNodeData(inode);
                var outputPort = port.SearchPortData(outputNode,output); 
                Conexions.Instance.AddConexion(this,port,outputNode,outputPort);
                Debug.Log($"Conexión añadida: {Type} ({Id}) puerto {port.Id} -> {outputNode.Type} ({outputNode.Id}) puerto {outputPort?.Id}");
            }
        }
    }
}
