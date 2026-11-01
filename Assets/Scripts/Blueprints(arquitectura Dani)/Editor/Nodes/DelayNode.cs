using Blueprints_arquitectura_Dani_.Editor.Nodes;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes
{
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