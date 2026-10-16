using System;
using Unity.GraphToolkit.Editor;

[Serializable]
public class IfNode : Node
{
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort("Input").WithConnectorUI(PortConnectorUI.Arrowhead).Build();
        context.AddInputPort<bool>("condition").WithDisplayName("Variable").Build();

        context.AddOutputPort("True").WithConnectorUI(PortConnectorUI.Arrowhead).Build();
        context.AddOutputPort("False").WithConnectorUI(PortConnectorUI.Arrowhead).Build();
    }
}