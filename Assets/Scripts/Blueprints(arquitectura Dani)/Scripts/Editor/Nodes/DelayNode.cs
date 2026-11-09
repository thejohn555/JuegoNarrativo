using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Unity.GraphToolkit.Editor;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes
{
    [Node(categoryPath:"Nodes/General/Flow")]
    public class DelayNode:BlueprintNode
    {
        public const string TimeOption="Time Option";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("In").Build();
            context.AddOutputPort("Out").Build();
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<float>(TimeOption).Build();
        }
    }
}