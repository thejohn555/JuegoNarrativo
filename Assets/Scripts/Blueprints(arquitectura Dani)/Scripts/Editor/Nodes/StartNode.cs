using System;
using Blueprints_arquitectura_Dani_.Runtime;
using Unity.GraphToolkit.Editor;

namespace Blueprints_arquitectura_Dani_.Editor.Nodes
{
    [Serializable]
    [Node(categoryPath:"Nodes/General/Start")]
    public class StartNode : BlueprintNode,IStartNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort(ExecutionPortDefaultName)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}