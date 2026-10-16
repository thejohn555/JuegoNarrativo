using System;
using Unity.GraphToolkit.Editor;

namespace Blueprints_arquitectura_Dani_.Editor.Nodes
{
    [Serializable]
    public class ConditionNode : BlueprintNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            const int maxTransitions = 2;
            const string TransitionPortPrefix = "Transition";
            context.AddInputPort(ExecutionPortDefaultName)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            for (int i = 0; i < maxTransitions; i++)
            {
                context.AddOutputPort($"{TransitionPortPrefix}{i}")
                    .WithDisplayName($"Transition {i+1}")
                    .WithConnectorUI(PortConnectorUI.Arrowhead)
                    .Build();
            }
        }
    }
}